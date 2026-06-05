using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Tuta.WinUI.ViewModels;

namespace Tuta.WinUI.Views;

public sealed partial class MailPage : Page
{
    public MailListViewModel ViewModel { get; }

    public MailPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<MailListViewModel>();
        DataContext = ViewModel;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (!ViewModel.LoadCommand.IsRunning)
        {
            ViewModel.LoadCommand.Execute(null);
        }
    }
}
