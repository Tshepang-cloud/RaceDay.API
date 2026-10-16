using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Route
{
    [Key]
    public int RouteId { get; set; }
    public int CategoryId { get; set; }
    public string? RouteDescription { get; set; }

    [Required, MaxLength(150)]
    public string StartPoint { get; set; }

    [Required, MaxLength(150)]
    public string EndPoint { get; set; }

    public int ElevationGainM { get; set; }

    // Navigation
    [ForeignKey("CategoryId")]
    public Category Category { get; set; }
}