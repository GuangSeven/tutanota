namespace Tuta.Core.Domain;

public sealed record CalendarEvent(
    string Id,
    string CalendarId,
    string Title,
    DateTimeOffset Start,
    DateTimeOffset End,
    bool IsAllDay,
    string? Location,
    string? Notes
);
