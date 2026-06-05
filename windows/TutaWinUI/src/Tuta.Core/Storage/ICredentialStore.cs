namespace Tuta.Core.Storage;

public interface ICredentialStore
{
    Task<StoredCredentials?> LoadAsync(CancellationToken cancellationToken);
    Task SaveAsync(StoredCredentials credentials, CancellationToken cancellationToken);
}
