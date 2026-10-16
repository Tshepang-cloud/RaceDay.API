using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }
    public int EventId { get; set; }

    [Required, MaxLength(50)]
    public string CategoryName { get; set; }

    public decimal DistanceKm { get; set; }
    public decimal EntryFee { get; set; }
    public int MaxParticipants { get; set; }

    // Navigation
    [ForeignKey("EventId")]
    public Event Event { get; set; }
    public Route? Route { get; set; }
}