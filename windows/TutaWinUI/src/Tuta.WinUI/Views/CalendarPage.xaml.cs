using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Tuta.WinUI.ViewModels;

namespace Tuta.WinUI.Views;

public sealed partial class CalendarPage : Page
{
    public CalendarViewModel ViewModel { get; }

    public CalendarPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<CalendarViewModel>();
        DataContext = ViewModel;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (Calendar.SelectedDates.Count == 0)
        {
            Calendar.SelectedDates.Add(DateTimeOffset.Now);
        }

        if (!ViewModel.LoadCommand.IsRunning)
        {
            ViewModel.LoadCommand.Execute(null);
        }
    }

    private void OnSelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
    {
        var date = args.AddedDates.FirstOrDefault();
        if (date != null)
        {
            ViewModel.SelectedDate = date.Value;
        }
    }
}
