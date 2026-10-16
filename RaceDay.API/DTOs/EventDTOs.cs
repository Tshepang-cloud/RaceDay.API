namespace RaceDay.API.DTOs;

public record EventDto(int EventId, string EventName, string? Description, DateTime EventDate, string Location, string Province);
public record CreateEventDto(string EventName, string? Description, DateTime EventDate, string Location, string Province);
public record UpdateEventDto(string EventName, string? Description, DateTime EventDate, string Location, string Province);