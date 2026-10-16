using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using System.Security.Claims;

namespace RaceDay.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly RaceDayDbContext _context;

    public EventsController(RaceDayDbContext context) => _context = context;

    /// <summary>Lists all upcoming events.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEvents()
    {
        var events = await _context.Events
            .Select(e => new EventDto(e.EventId, e.EventName, e.Description, e.EventDate, e.Location, e.Province))
            .ToListAsync();
        return Ok(events);
    }

    /// <summary>Returns full details of one event.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEvent(int id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null) return NotFound();
        return Ok(new EventDto(ev.EventId, ev.EventName, ev.Description, ev.EventDate, ev.Location, ev.Province));
    }

    /// <summary>Creates a new event owned by the logged-in organiser.</summary>
    [HttpPost]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEvent(CreateEventDto request)
    {
        var organiserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var newEvent = new Event
        {
            OrganiserId = organiserId,
            EventName = request.EventName,
            Description = request.Description,
            EventDate = request.EventDate,
            Location = request.Location,
            Province = request.Province
        };

        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEvent), new { id = newEvent.EventId }, newEvent);
    }

    /// <summary>Updates an event owned by the logged-in organiser.</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEvent(int id, UpdateEventDto request)
    {
        var organiserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var ev = await _context.Events.FindAsync(id);

        if (ev == null) return NotFound();
        if (ev.OrganiserId != organiserId) return Forbid();

        ev.EventName = request.EventName;
        ev.Description = request.Description;
        ev.EventDate = request.EventDate;
        ev.Location = request.Location;
        ev.Province = request.Province;

        await _context.SaveChangesAsync();
        return Ok(ev);
    }

    /// <summary>Deletes an event owned by the logged-in organiser.</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var organiserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var ev = await _context.Events.FindAsync(id);

        if (ev == null) return NotFound();
        if (ev.OrganiserId != organiserId) return Forbid();

        _context.Events.Remove(ev);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}