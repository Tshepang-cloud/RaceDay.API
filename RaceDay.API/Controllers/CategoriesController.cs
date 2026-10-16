using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using System.Security.Claims;

namespace RaceDay.API.Controllers;

[Route("api")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayDbContext _context;
    public CategoriesController(RaceDayDbContext context) => _context = context;

    /// <summary>Lists all categories for an event.</summary>
    [HttpGet("events/{eventId}/categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategories(int eventId)
    {
        var eventExists = await _context.Events.AnyAsync(e => e.EventId == eventId);
        if (!eventExists) return NotFound();

        var categories = await _context.Categories
            .Where(c => c.EventId == eventId)
            .Select(c => new CategoryDto(c.CategoryId, c.CategoryName, c.DistanceKm, c.EntryFee, c.MaxParticipants))
            .ToListAsync();

        return Ok(categories);
    }

    /// <summary>Adds a new category to an event.</summary>
    [HttpPost("events/{eventId}/categories")]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCategory(int eventId, CreateCategoryDto request)
    {
        var organiserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var ev = await _context.Events.FindAsync(eventId);

        if (ev == null) return NotFound();
        if (ev.OrganiserId != organiserId) return Forbid();

        var category = new Category
        {
            EventId = eventId,
            CategoryName = request.CategoryName,
            DistanceKm = request.DistanceKm,
            EntryFee = request.EntryFee,
            MaxParticipants = request.MaxParticipants
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategories), new { eventId = eventId }, category);
    }

    /// <summary>Returns route info for a specific category.</summary>
    [HttpGet("categories/{id}/route")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoute(int id)
    {
        var route = await _context.Routes.FirstOrDefaultAsync(r => r.CategoryId == id);
        if (route == null) return NotFound();
        return Ok(new RouteDto(route.RouteDescription, route.StartPoint, route.EndPoint, route.ElevationGainM));
    }
}