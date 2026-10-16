namespace RaceDay.API.DTOs;

public record RegisterDto(string FullName, string Email, string Password, string Role, string? PhoneNumber);
public record LoginDto(string Email, string Password);
public record AuthResponseDto(string Token, string Role, int UserId);