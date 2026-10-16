using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Result
{
    [Key]
    public int ResultId { get; set; }
    public int EnrolmentId { get; set; }
    public TimeSpan? FinishTime { get; set; }
    public int? Position { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; }

    public DateTime RecordedAt { get; set; }

    // Navigation
    [ForeignKey("EnrolmentId")]
    public Enrolment Enrolment { get; set; }
}