namespace RaceDay.API.DTOs;

public record UserProfileDto(int UserId, string FullName, string Email, string Role, string? PhoneNumber);
public record UpdateUserProfileDto(string FullName, string? PhoneNumber);