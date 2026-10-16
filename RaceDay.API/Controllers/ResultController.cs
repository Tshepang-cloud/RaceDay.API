using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[Route("api")]
[ApiController]
[Authorize]
public class ResultsController : ControllerBase
{
    private readonly RaceDayDbContext _context;
    public ResultsController(RaceDayDbContext context) => _context = context;

    /// <summary>Captures a participant's finish result for their enrolment.</summary>
    [HttpPost("enrolments/{id}/result")]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateResult(int id, CreateResultDto request)
    {
        var enrolment = await _context.Enrolments.FindAsync(id);
        if (enrolment == null) return NotFound();

        var existingResult = await _context.Results.AnyAsync(r => r.EnrolmentId == id);
        if (existingResult) return Conflict(new { message = "Result already recorded for this enrolment" });

        var result = new Result
        {
            EnrolmentId = id,
            FinishTime = request.FinishTime,
            Position = request.Position,
            Status = request.Status
        };

        _context.Results.Add(result);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(CreateResult), new { id = result.ResultId }, result);
    }

    /// <summary>Returns all results for an event.</summary>
    [HttpGet("events/{id}/results")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEventResults(int id)
    {
        var eventExists = await _context.Events.AnyAsync(e => e.EventId == id);
        if (!eventExists) return NotFound();

        var results = await _context.Results
            .Include(r => r.Enrolment)
            .ThenInclude(e => e.Participant)
            .Include(r => r.Enrolment)
            .ThenInclude(e => e.Category)
            .Where(r => r.Enrolment.Category.EventId == id)
            .Select(r => new { r.ResultId, r.FinishTime, r.Position, r.Status, ParticipantName = r.Enrolment.Participant.FullName, CategoryName = r.Enrolment.Category.CategoryName })
            .ToListAsync();

        return Ok(results);
    }
}