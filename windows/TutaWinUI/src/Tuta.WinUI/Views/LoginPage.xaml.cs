using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Tuta.WinUI.ViewModels;

namespace Tuta.WinUI.Views;

public sealed partial class LoginPage : Page
{
    public LoginViewModel ViewModel { get; }
    public event EventHandler? UnlockSucceeded;

    public LoginPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<LoginViewModel>();
        ViewModel.Unlocked += OnUnlocked;
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

    private void OnUnlocked(object? sender, EventArgs e)
    {
        UnlockSucceeded?.Invoke(this, EventArgs.Empty);
    }

    private void OnPasswordChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.Password = PasswordBox.Password;
    }

    private void OnConfirmPasswordChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.ConfirmPassword = ConfirmBox.Password;
    }
}
