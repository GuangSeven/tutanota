using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Tuta.WinUI.ViewModels;

namespace Tuta.WinUI.Views;

public sealed partial class ContactsPage : Page
{
    public ContactsViewModel ViewModel { get; }

    public ContactsPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<ContactsViewModel>();
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
