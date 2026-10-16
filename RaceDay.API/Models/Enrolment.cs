using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Enrolment
{
    [Key]
    public int EnrolmentId { get; set; }
    public int ParticipantId { get; set; }
    public int CategoryId { get; set; }
    public DateTime EnrolmentDate { get; set; }

    [Required, MaxLength(10)]
    public string BibNumber { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } 

    // Navigation
    [ForeignKey("ParticipantId")]
    public User Participant { get; set; }
    [ForeignKey("CategoryId")]
    public Category Category { get; set; }
    public Result? Result { get; set; }
}