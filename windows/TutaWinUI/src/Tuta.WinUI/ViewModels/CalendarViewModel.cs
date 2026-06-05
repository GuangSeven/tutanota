using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tuta.Core.Domain;
using Tuta.Core.Services;

namespace Tuta.WinUI.ViewModels;

public sealed partial class CalendarViewModel : ObservableObject
{
    private readonly ICalendarService _calendarService;

    public ObservableCollection<CalendarEvent> Events { get; } = new();

    [ObservableProperty]
    private DateTimeOffset selectedDate = DateTimeOffset.UtcNow.Date;

    [ObservableProperty]
    private bool isBusy;

    public IAsyncRelayCommand LoadCommand { get; }

    public CalendarViewModel(ICalendarService calendarService)
    {
        _calendarService = calendarService;
        LoadCommand = new AsyncRelayCommand(LoadAsync);
    }

    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var events = await _calendarService.GetEventsForDayAsync(DateOnly.FromDateTime(SelectedDate.DateTime), CancellationToken.None);
            Replace(Events, events);
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSelectedDateChanged(DateTimeOffset value)
    {
        _ = LoadAsync();
    }

    private static void Replace<T>(ObservableCollection<T> target, IEnumerable<T> items)
    {
        target.Clear();
        foreach (var item in items)
        {
            target.Add(item);
        }
    }
}
