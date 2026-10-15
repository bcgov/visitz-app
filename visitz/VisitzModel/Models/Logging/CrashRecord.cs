namespace VisitzModel.Models.Logging;

public record CrashRecord(
    Guid Id,
    DateTimeOffset TimestampUtc,
    string Platform,
    string ExceptionType,
    string Message,
    string ExceptionText
);
