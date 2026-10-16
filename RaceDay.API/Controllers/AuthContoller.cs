using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly RaceDayDbContext _context;
    private readonly TokenService _tokenService;

    public AuthController(RaceDayDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    /// <summary>Registers a new user as an Organiser or Participant.</summary>
    /// <response code="201">Returns the newly created user</response>
    /// <response code="400">If the role is invalid</response>
    /// <response code="409">If the email is already in use</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return Conflict(new { message = "Email already in use" });

        if (request.Role != "Organiser" && request.Role != "Participant")
            return BadRequest(new { message = "Invalid role" });

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            PhoneNumber = request.PhoneNumber
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Register), new { id = user.UserId }, new { user.UserId, user.Email, user.Role });
    }

    /// <summary>Authenticates a user and returns a JWT token.</summary>
    /// <response code="200">Returns the JWT token and role</response>
    /// <response code="401">If credentials are invalid</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid credentials" });

        var token = _tokenService.CreateToken(user);
        return Ok(new AuthResponseDto(token, user.Role, user.UserId));
    }
}