using System.IO;
using System.Text.Json;

namespace Ametrin.Serializer;

public interface IAmetrinSerializable<TSelf> where TSelf : IAmetrinSerializable<TSelf>
{
    public static abstract void Serialize(TSelf self, IAmetrinWriter writer);
    public static abstract TSelf Deserialize(IAmetrinReader reader);
}
