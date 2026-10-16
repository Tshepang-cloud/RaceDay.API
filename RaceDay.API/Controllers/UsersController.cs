using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using System.Security.Claims;

namespace RaceDay.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly RaceDayDbContext _context;
    public UsersController(RaceDayDbContext context) => _context = context;

    /// <summary>Returns the logged-in user's profile.</summary>
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProfile()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        return Ok(new UserProfileDto(user.UserId, user.FullName, user.Email, user.Role, user.PhoneNumber));
    }

    /// <summary>Updates the logged-in user's profile details.</summary>
    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfile(UpdateUserProfileDto request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;
        await _context.SaveChangesAsync();

        return Ok(new UserProfileDto(user.UserId, user.FullName, user.Email, user.Role, user.PhoneNumber));
    }

    /// <summary>Returns the logged-in participant's personal performance history.</summary>
    [HttpGet("me/history")]
    [Authorize(Roles = "Participant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetHistory()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var history = await _context.Enrolments
            .Include(e => e.Result)
            .Where(e => e.ParticipantId == userId && e.Result != null)
            .Select(e => new { e.EnrolmentId, e.BibNumber, e.Result!.FinishTime, e.Result.Position, e.Result.Status })
            .ToListAsync();

        return Ok(history);
    }
}