using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Event
{
    [Key]
    public int EventId { get; set; }

    public int OrganiserId { get; set; }

    [Required, MaxLength(150)]
    public string EventName { get; set; }
    public string? Description { get; set; }

    public DateTime EventDate { get; set; }

    [Required, MaxLength(150)]
    public string Location { get; set; }

    [Required, MaxLength(50)]
    public string Province { get; set; }

    public DateTime CreatedAt { get; set; }
    [ForeignKey("OrganiserId")]
    public User Organiser { get; set; }

}