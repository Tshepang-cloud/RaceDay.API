using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;

namespace RaceDay.API.Controllers;

[Route("api")]
[ApiController]
public class WeatherController : ControllerBase
{
    private readonly RaceDayDbContext _context;
    public WeatherController(RaceDayDbContext context) => _context = context;

    /// <summary>Fetches mock live weather forecast for the event's location.</summary>
    [HttpGet("events/{id}/weather")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWeather(int id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null) return NotFound();

        // Mock response simulating an external weather API call
        var weather = new
        {
            EventId = id,
            Location = ev.Location,
            Temperature = "24°C",
            Wind = "32 km/h SE",
            Conditions = "Clear and Dry",
            Humidity = "68%",
            ForecastDate = DateTime.UtcNow.ToString("yyyy-MM-dd")
        };

        return Ok(weather);
    }
}