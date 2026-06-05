using System.Text.Json;
using Tuta.Core.Storage;

namespace Tuta.Infrastructure.Storage;

public sealed class JsonEntitySerializer : IEntitySerializer
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false,
    };

    public byte[] Serialize<T>(T value)
    {
        return JsonSerializer.SerializeToUtf8Bytes(value, Options);
    }

    public T Deserialize<T>(byte[] payload)
    {
        return JsonSerializer.Deserialize<T>(payload, Options) ?? throw new InvalidOperationException("Failed to deserialize payload.");
    }
}
