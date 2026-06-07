using Microsoft.UI.Xaml;
using Tuta.WinUI.Views;
using Windows.Graphics;

namespace Tuta.WinUI;

public sealed partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        AppWindow.Resize(new SizeInt32(420, 520));
        RootFrame.Navigate(typeof(LoginPage));
        RootFrame.Navigated += OnNavigated;
    }

    private void OnNavigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        if (RootFrame.Content is LoginPage page)
        {
            page.UnlockSucceeded += OnUnlockSucceeded;
        }
    }

    private void OnUnlockSucceeded(object? sender, EventArgs e)
    {
        var mainWindow = new MainWindow();
        mainWindow.Activate();
        Close();
    }
}
