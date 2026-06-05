using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Tuta.WinUI.ViewModels;

namespace Tuta.WinUI.Views;

public sealed partial class ComposePage : Page
{
    public ComposeViewModel ViewModel { get; }

    public ComposePage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<ComposeViewModel>();
        DataContext = ViewModel;
    }
}
