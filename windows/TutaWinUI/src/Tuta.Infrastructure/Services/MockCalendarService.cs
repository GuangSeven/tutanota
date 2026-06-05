using Tuta.Core.Domain;
using Tuta.Core.Services;
using Tuta.Infrastructure.Storage;

namespace Tuta.Infrastructure.Services;

public sealed class MockCalendarService : ICalendarService
{
    private const string CalendarId = "primary";
    private readonly EncryptedEntityRepository<CalendarEvent> _repo;
    private readonly SemaphoreSlim _seedLock = new(1, 1);
    private bool _seeded;

    public MockCalendarService(EncryptedEntityRepository<CalendarEvent> repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyList<CalendarEvent>> GetEventsAsync(DateTimeOffset start, DateTimeOffset end, CancellationToken cancellationToken)
    {
        await EnsureSeededAsync(cancellationToken);
        var all = await _repo.ListAsync(CalendarId, cancellationToken);
        return all.Where(e => e.Start < end && e.End > start).ToList();
    }

    public Task<IReadOnlyList<CalendarEvent>> GetEventsForDayAsync(DateOnly day, CancellationToken cancellationToken)
    {
        var start = day.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var end = day.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
        return GetEventsAsync(new DateTimeOffset(start), new DateTimeOffset(end), cancellationToken);
    }

    public async Task<IReadOnlyList<CalendarEvent>> GetMonthAsync(int year, int month, CancellationToken cancellationToken)
    {
        var start = new DateTimeOffset(new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc));
        var end = start.AddMonths(1);
        return await GetEventsAsync(start, end, cancellationToken);
    }

    private async Task EnsureSeededAsync(CancellationToken cancellationToken)
    {
        if (_seeded)
        {
            return;
        }

        await _seedLock.WaitAsync(cancellationToken);
        try
        {
            if (_seeded)
            {
                return;
            }

            foreach (var ev in SampleDataFactory.CalendarEvents(CalendarId))
            {
                await _repo.SaveAsync(ev.Id, CalendarId, ev, cancellationToken);
            }

            _seeded = true;
        }
        finally
        {
            _seedLock.Release();
        }
    }
}
