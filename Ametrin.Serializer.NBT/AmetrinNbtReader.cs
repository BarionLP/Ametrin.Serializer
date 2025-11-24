namespace Ametrin.Serializer.NBT;

public sealed class AmetrinNbtReader(CompoundTag tag) : IAmetrinReader
{
    private readonly CompoundTag tag = tag;

    public Result<bool, DeserializationError> TryReadBooleanProperty(ReadOnlySpan<char> name) => TryReadTag<SbyteTag>(name, "Boolean").Map(static tag => tag.Value is 1);
    public Result<byte[], DeserializationError> TryReadBytesProperty(ReadOnlySpan<char> name) => TryReadTag<ByteArrayTag>(name, "byte[]").Map(static tag => tag.Value);
    public Result<DateTime, DeserializationError> TryReadDateTimeProperty(ReadOnlySpan<char> name) => TryReadTag<LongTag>(name, "DateTime").Map(static tag => new DateTime(tag.Value));
    public Result<double, DeserializationError> TryReadDoubleProperty(ReadOnlySpan<char> name) => TryReadTag<DoubleTag>(name, "Double").Map(static tag => tag.Value);
    public Result<int, DeserializationError> TryReadInt32Property(ReadOnlySpan<char> name) => TryReadTag<IntTag>(name, "Int32").Map(static tag => tag.Value);
    public Result<float, DeserializationError> TryReadSingleProperty(ReadOnlySpan<char> name) => TryReadTag<FloatTag>(name, "Single").Map(static tag => tag.Value);
    public Result<string, DeserializationError> TryReadStringProperty(ReadOnlySpan<char> name) => TryReadTag<StringTag>(name, "String").Map(static tag => tag.Value);
    public Result<T, DeserializationError> TryReadObjectProperty<T>(ReadOnlySpan<char> name) where T : IAmetrinSerializable<T>
        => TryReadTag<CompoundTag>(name, nameof(T)).Map(static tag => T.TryDeserialize(new AmetrinNbtReader(tag)));

    public void ReadStartObject()
    {
    }
    public void ReadEndObject()
    {
    }

    public bool ReadBooleanProperty(ReadOnlySpan<char> name)
    {
        throw new NotImplementedException();
    }
    public byte[] ReadBytesProperty(ReadOnlySpan<char> name)
    {
        throw new NotImplementedException();
    }
    public DateTime ReadDateTimeProperty(ReadOnlySpan<char> name)
    {
        throw new NotImplementedException();
    }
    public double ReadDoubleProperty(ReadOnlySpan<char> name)
    {
        throw new NotImplementedException();
    }
    public int ReadInt32Property(ReadOnlySpan<char> name)
    {
        throw new NotImplementedException();
    }
    public float ReadSingleProperty(ReadOnlySpan<char> name)
    {
        throw new NotImplementedException();
    }
    public string ReadStringProperty(ReadOnlySpan<char> name) => TryReadStringProperty(name).Or(static e => e.Throw<string>());
    public T ReadObjectProperty<T>(ReadOnlySpan<char> name) where T : IAmetrinSerializable<T>
    {
        throw new NotImplementedException();
    }

    private readonly Dictionary<string, Tag>.AlternateLookup<ReadOnlySpan<char>> lookup = tag.Value.GetAlternateLookup<ReadOnlySpan<char>>();
    private Result<TTag, DeserializationError> TryReadTag<TTag>(ReadOnlySpan<char> name, string typeName) where TTag : Tag
    {
        if (!lookup.TryGetValue(name, out var inner))
        {
            return DeserializationError.CreatePropertyNotFound(name.ToString());
        }

        if (inner is not TTag tag)
        {
            return DeserializationError.CreateInvalidPropertyType(name.ToString(), typeName);
        }

        return tag;
    }
}
