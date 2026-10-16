namespace RaceDay.API.DTOs;

public record EnrolmentDto(int EnrolmentId, string BibNumber, string Status, DateTime EnrolmentDate, string CategoryName, string EventName);
public record CreateResultDto(TimeSpan? FinishTime, int? Position, string Status);