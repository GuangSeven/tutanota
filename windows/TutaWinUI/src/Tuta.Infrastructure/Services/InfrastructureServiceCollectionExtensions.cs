using Microsoft.Extensions.DependencyInjection;
using Tuta.Core.Crypto;
using Tuta.Core.Domain;
using Tuta.Core.Events;
using Tuta.Core.Services;
using Tuta.Core.Storage;
using Tuta.Infrastructure.Crypto;
using Tuta.Infrastructure.Storage;

namespace Tuta.Infrastructure.Services;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddTutaInfrastructure(this IServiceCollection services, string dataRoot)
    {
        var dbPath = Path.Combine(dataRoot, "tuta-local.db");
        var keyPath = Path.Combine(dataRoot, "keys.json");
        var credentialPath = Path.Combine(dataRoot, "credentials.json");

        services.AddSingleton<ICryptoService, AesGcmCryptoService>();
        services.AddSingleton<IPasswordKeyDeriver, PasswordKeyDeriver>();
        services.AddSingleton<ICredentialStore>(_ => new FileCredentialStore(credentialPath));
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IMasterKeyProvider>(sp => (AuthService)sp.GetRequiredService<IAuthService>());
        services.AddSingleton<IKeyProtector, MasterKeyProtector>();
        services.AddSingleton<IKeyStore>(sp => new FileKeyStore(keyPath, sp.GetRequiredService<IKeyProtector>()));
        services.AddSingleton<IEntitySerializer, JsonEntitySerializer>();
        services.AddSingleton<ILocalStore>(_ => new SqliteLocalStore(dbPath));
        services.AddSingleton<IEntityUpdateHub, InMemoryEntityUpdateHub>();

        services.AddSingleton(sp => new EncryptedEntityRepository<MailSummary>(
            "MailSummary",
            "local",
            sp.GetRequiredService<ILocalStore>(),
            sp.GetRequiredService<IEntitySerializer>(),
            sp.GetRequiredService<ICryptoService>(),
            sp.GetRequiredService<IKeyStore>()));

        services.AddSingleton(sp => new EncryptedEntityRepository<MailMessage>(
            "MailMessage",
            "local",
            sp.GetRequiredService<ILocalStore>(),
            sp.GetRequiredService<IEntitySerializer>(),
            sp.GetRequiredService<ICryptoService>(),
            sp.GetRequiredService<IKeyStore>()));

        services.AddSingleton(sp => new EncryptedEntityRepository<MailDraft>(
            "MailDraft",
            "local",
            sp.GetRequiredService<ILocalStore>(),
            sp.GetRequiredService<IEntitySerializer>(),
            sp.GetRequiredService<ICryptoService>(),
            sp.GetRequiredService<IKeyStore>()));

        services.AddSingleton(sp => new EncryptedEntityRepository<Contact>(
            "Contact",
            "local",
            sp.GetRequiredService<ILocalStore>(),
            sp.GetRequiredService<IEntitySerializer>(),
            sp.GetRequiredService<ICryptoService>(),
            sp.GetRequiredService<IKeyStore>()));

        services.AddSingleton(sp => new EncryptedEntityRepository<CalendarEvent>(
            "CalendarEvent",
            "local",
            sp.GetRequiredService<ILocalStore>(),
            sp.GetRequiredService<IEntitySerializer>(),
            sp.GetRequiredService<ICryptoService>(),
            sp.GetRequiredService<IKeyStore>()));

        services.AddSingleton<IMailboxService, MockMailboxService>();
        services.AddSingleton<IMailService, MockMailService>();
        services.AddSingleton<IMailComposeService, MockMailComposeService>();
        services.AddSingleton<IContactsService, MockContactsService>();
        services.AddSingleton<ICalendarService, MockCalendarService>();

        return services;
    }
}
