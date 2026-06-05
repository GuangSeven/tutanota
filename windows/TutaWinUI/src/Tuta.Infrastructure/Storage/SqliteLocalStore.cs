using Microsoft.Data.Sqlite;
using Tuta.Core.Crypto;
using Tuta.Core.Storage;

namespace Tuta.Infrastructure.Storage;

public sealed class SqliteLocalStore : ILocalStore
{
    private readonly string _dbPath;
    private readonly SemaphoreSlim _mutex = new(1, 1);
    private bool _initialized;

    public SqliteLocalStore(string dbPath)
    {
        _dbPath = dbPath;
    }

    public async Task InitializeAsync(UserContext context, CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);
    }

    public async Task SaveAsync(EntityEnvelope envelope, CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);
        await _mutex.WaitAsync(cancellationToken);
        try
        {
            await using var connection = new SqliteConnection($"Data Source={_dbPath}");
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = @"
INSERT OR REPLACE INTO entities (
    entityType,
    entityId,
    listId,
    ownerGroupId,
    keyVersion,
    nonce,
    ciphertext,
    tag
) VALUES (
    $entityType,
    $entityId,
    $listId,
    $ownerGroupId,
    $keyVersion,
    $nonce,
    $ciphertext,
    $tag
);";

            BindEnvelope(command, envelope);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<EntityEnvelope?> LoadAsync(string entityType, string entityId, CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);
        await _mutex.WaitAsync(cancellationToken);
        try
        {
            await using var connection = new SqliteConnection($"Data Source={_dbPath}");
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = @"
SELECT entityType, entityId, listId, ownerGroupId, keyVersion, nonce, ciphertext, tag
FROM entities
WHERE entityType = $entityType AND entityId = $entityId;";
            command.Parameters.AddWithValue("$entityType", entityType);
            command.Parameters.AddWithValue("$entityId", entityId);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            return ReadEnvelope(reader);
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<IReadOnlyList<EntityEnvelope>> QueryListAsync(string entityType, string listId, CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);
        await _mutex.WaitAsync(cancellationToken);
        try
        {
            var result = new List<EntityEnvelope>();
            await using var connection = new SqliteConnection($"Data Source={_dbPath}");
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = @"
SELECT entityType, entityId, listId, ownerGroupId, keyVersion, nonce, ciphertext, tag
FROM entities
WHERE entityType = $entityType AND listId = $listId;";
            command.Parameters.AddWithValue("$entityType", entityType);
            command.Parameters.AddWithValue("$listId", listId);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(ReadEnvelope(reader));
            }

            return result;
        }
        finally
        {
            _mutex.Release();
        }
    }

    private static void BindEnvelope(SqliteCommand command, EntityEnvelope envelope)
    {
        command.Parameters.AddWithValue("$entityType", envelope.EntityType);
        command.Parameters.AddWithValue("$entityId", envelope.EntityId);
        command.Parameters.AddWithValue("$listId", envelope.ListId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$ownerGroupId", envelope.OwnerGroupId);
        command.Parameters.AddWithValue("$keyVersion", envelope.KeyVersion);
        command.Parameters.AddWithValue("$nonce", envelope.Payload.Nonce);
        command.Parameters.AddWithValue("$ciphertext", envelope.Payload.Ciphertext);
        command.Parameters.AddWithValue("$tag", envelope.Payload.Tag);
    }

    private static EntityEnvelope ReadEnvelope(SqliteDataReader reader)
    {
        var entityType = reader.GetString(0);
        var entityId = reader.GetString(1);
        var listId = reader.IsDBNull(2) ? null : reader.GetString(2);
        var ownerGroupId = reader.GetString(3);
        var keyVersion = reader.GetInt32(4);
        var nonce = (byte[])reader[5];
        var ciphertext = (byte[])reader[6];
        var tag = (byte[])reader[7];

        return new EntityEnvelope(
            entityType,
            entityId,
            listId,
            ownerGroupId,
            new EncryptedPayload(nonce, ciphertext, tag),
            keyVersion);
    }

    private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
    {
        if (_initialized)
        {
            return;
        }

        await _mutex.WaitAsync(cancellationToken);
        try
        {
            if (_initialized)
            {
                return;
            }

            var directory = Path.GetDirectoryName(_dbPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await using var connection = new SqliteConnection($"Data Source={_dbPath}");
            await connection.OpenAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = @"
CREATE TABLE IF NOT EXISTS entities (
    entityType TEXT NOT NULL,
    entityId TEXT NOT NULL,
    listId TEXT NULL,
    ownerGroupId TEXT NOT NULL,
    keyVersion INTEGER NOT NULL,
    nonce BLOB NOT NULL,
    ciphertext BLOB NOT NULL,
    tag BLOB NOT NULL,
    PRIMARY KEY (entityType, entityId)
);";
            await command.ExecuteNonQueryAsync(cancellationToken);
            _initialized = true;
        }
        finally
        {
            _mutex.Release();
        }
    }
}
