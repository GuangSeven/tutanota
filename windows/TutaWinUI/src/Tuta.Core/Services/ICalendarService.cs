using Tuta.Core.Domain;

namespace Tuta.Core.Services;

public interface ICalendarService
{
    Task<IReadOnlyList<CalendarEvent>> GetEventsAsync(DateTimeOffset start, DateTimeOffset end, CancellationToken cancellationToken);
    Task<IReadOnlyList<CalendarEvent>> GetEventsForDayAsync(DateOnly day, CancellationToken cancellationToken);
    Task<IReadOnlyList<CalendarEvent>> GetMonthAsync(int year, int month, CancellationToken cancellationToken);
}
