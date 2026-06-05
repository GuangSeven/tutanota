using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Tuta.WinUI.Views;

namespace Tuta.WinUI;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        NavView.SelectedItem = NavView.MenuItems[0];
        ContentFrame.Navigate(typeof(MailPage));
    }

    private void OnNavSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is not NavigationViewItem item)
        {
            return;
        }

        switch (item.Tag?.ToString())
        {
            case "mail":
                ContentFrame.Navigate(typeof(MailPage));
                break;
            case "compose":
                ContentFrame.Navigate(typeof(ComposePage));
                break;
            case "contacts":
                ContentFrame.Navigate(typeof(ContactsPage));
                break;
            case "calendar":
                ContentFrame.Navigate(typeof(CalendarPage));
                break;
        }
    }
}
