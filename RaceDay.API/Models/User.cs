using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required, MaxLength(100)]
    public string FullName { get; set; }

    [Required, MaxLength(150)]
    public string Email { get; set; } 

    [Required, MaxLength(255)]
    public string PasswordHash { get; set; }

    [Required, MaxLength(20)]
    public string Role { get; set; } // Organiser or Participant

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    public DateTime CreatedAt { get; set; }

}