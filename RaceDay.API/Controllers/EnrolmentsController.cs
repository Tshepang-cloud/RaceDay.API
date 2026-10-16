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
[Authorize]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayDbContext _context;
    public EnrolmentsController(RaceDayDbContext context) => _context = context;

    /// <summary>Enrols the logged-in participant into a category.</summary>
    [HttpPost("categories/{id}/enrolments")]
    [Authorize(Roles = "Participant")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Enrol(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var category = await _context.Categories.Include(c => c.Enrolments).FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null) return NotFound();
        if (category.Enrolments.Count >= category.MaxParticipants) return BadRequest(new { message = "Category is full" });

        var existing = await _context.Enrolments.AnyAsync(e => e.ParticipantId == userId && e.CategoryId == id);
        if (existing) return Conflict(new { message = "Already enrolled in this category" });

        var enrolment = new Enrolment
        {
            ParticipantId = userId,
            CategoryId = id,
            BibNumber = $"BIB-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            Status = "Confirmed"
        };

        _context.Enrolments.Add(enrolment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Enrol), new { id = enrolment.EnrolmentId }, enrolment);
    }

    /// <summary>Lists the logged-in participant's own enrolments.</summary>
    [HttpGet("users/me/enrolments")]
    [Authorize(Roles = "Participant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyEnrolments()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var enrolments = await _context.Enrolments
            .Include(e => e.Category)
            .ThenInclude(c => c.Event)
            .Where(e => e.ParticipantId == userId)
            .Select(e => new EnrolmentDto(e.EnrolmentId, e.BibNumber, e.Status, e.EnrolmentDate, e.Category.CategoryName, e.Category.Event.EventName))
            .ToListAsync();

        return Ok(enrolments);
    }

    /// <summary>Lists all participants enrolled in a category (organiser roster view).</summary>
    [HttpGet("categories/{id}/enrolments")]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCategoryEnrolments(int id)
    {
        var enrolments = await _context.Enrolments
            .Include(e => e.Participant)
            .Where(e => e.CategoryId == id)
            .Select(e => new { e.EnrolmentId, e.BibNumber, e.Status, e.EnrolmentDate, ParticipantName = e.Participant.FullName })
            .ToListAsync();

        return Ok(enrolments);
    }
}