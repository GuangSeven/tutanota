namespace Tuta.Core.Storage;

public interface IEntitySerializer
{
    byte[] Serialize<T>(T value);
    T Deserialize<T>(byte[] payload);
}
