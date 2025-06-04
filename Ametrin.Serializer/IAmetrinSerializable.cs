using Ametrin.Optional;

namespace Ametrin.Serializer;

public interface IAmetrinSerializable<TSelf> where TSelf : IAmetrinSerializable<TSelf>
{
    public static abstract ErrorState Serialize(TSelf self, IAmetrinWriter writer);
    public static abstract TSelf Deserialize(IAmetrinReader reader);
    public static abstract Result<TSelf, DeserializationError> TryDeserialize(IAmetrinReader reader);
}

public interface ITypedAmetrinSerializable<TSelf> : IAmetrinSerializable<TSelf> where TSelf : ITypedAmetrinSerializable<TSelf>;