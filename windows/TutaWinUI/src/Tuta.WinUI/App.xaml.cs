using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Tuta.Core.Storage;
using Tuta.Infrastructure.Services;
using Tuta.WinUI.ViewModels;

namespace Tuta.WinUI;

public sealed partial class App : Application
{
    public static IServiceProvider Services { get; } = ConfigureServices();

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var window = new LoginWindow();
        window.Activate();
        _ = InitializeAsync();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        var dataRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TutaWinUI");
        services.AddTutaInfrastructure(dataRoot);

        services.AddSingleton<MailListViewModel>();
        services.AddSingleton<ComposeViewModel>();
        services.AddSingleton<ContactsViewModel>();
        services.AddSingleton<CalendarViewModel>();
        services.AddSingleton<LoginViewModel>();

        return services.BuildServiceProvider();
    }

    private static async Task InitializeAsync()
    {
        var store = Services.GetRequiredService<ILocalStore>();
        await store.InitializeAsync(new UserContext("local-user", "device-001"), CancellationToken.None);
    }
}
