namespace RaceDay.API.DTOs;

public record CategoryDto(int CategoryId, string CategoryName, decimal DistanceKm, decimal EntryFee, int MaxParticipants);
public record CreateCategoryDto(string CategoryName, decimal DistanceKm, decimal EntryFee, int MaxParticipants);
public record RouteDto(string? RouteDescription, string StartPoint, string EndPoint, int ElevationGainM);