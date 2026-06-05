namespace Tuta.Core.Services;

public interface IAuthService
{
    event EventHandler<AuthState>? AuthStateChanged;
    Task<AuthState> GetStateAsync(CancellationToken cancellationToken);
    Task<bool> IsPasswordSetAsync(CancellationToken cancellationToken);
    Task SetPasswordAsync(string password, CancellationToken cancellationToken);
    Task<bool> SignInAsync(string password, CancellationToken cancellationToken);
    Task SignOutAsync(CancellationToken cancellationToken);
}
