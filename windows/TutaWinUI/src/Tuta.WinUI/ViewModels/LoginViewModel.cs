using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tuta.Core.Services;

namespace Tuta.WinUI.ViewModels;

public sealed partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private bool isFirstRun;

    public bool IsUnlockEnabled => !IsFirstRun;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private string status = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    public IAsyncRelayCommand LoadCommand { get; }
    public IAsyncRelayCommand UnlockCommand { get; }
    public IAsyncRelayCommand SetPasswordCommand { get; }

    public event EventHandler? Unlocked;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        LoadCommand = new AsyncRelayCommand(LoadAsync);
        UnlockCommand = new AsyncRelayCommand(UnlockAsync);
        SetPasswordCommand = new AsyncRelayCommand(SetPasswordAsync);
    }

    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            IsFirstRun = !await _authService.IsPasswordSetAsync(CancellationToken.None);
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnIsFirstRunChanged(bool value)
    {
        OnPropertyChanged(nameof(IsUnlockEnabled));
    }

    private async Task UnlockAsync()
    {
        Status = string.Empty;
        IsBusy = true;
        try
        {
            var ok = await _authService.SignInAsync(Password, CancellationToken.None);
            if (!ok)
            {
                Status = "Invalid password.";
                return;
            }

            Status = string.Empty;
            Unlocked?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SetPasswordAsync()
    {
        Status = string.Empty;
        IsBusy = true;
        try
        {
            if (Password.Length < 8)
            {
                Status = "Password must be at least 8 characters.";
                return;
            }

            if (!string.Equals(Password, ConfirmPassword, StringComparison.Ordinal))
            {
                Status = "Passwords do not match.";
                return;
            }

            await _authService.SetPasswordAsync(Password, CancellationToken.None);
            IsFirstRun = false;
            Status = string.Empty;
            Unlocked?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
