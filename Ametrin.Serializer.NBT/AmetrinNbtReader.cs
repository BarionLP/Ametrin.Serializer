namespace Ametrin.Serializer.NBT;

public sealed class AmetrinNbtReader(CompoundTag tag) : IAmetrinReader
{
    private readonly CompoundTag tag = tag;

    public Result<bool, DeserializationError> TryReadBooleanProperty(ReadOnlySpan<char> name) => TryReadTag<SbyteTag>(name, "Boolean")
        .Require(name, static (tag, _) => tag.Value is 0 or 1, (tag, name) => DeserializationError.CreateInvalidValue(name.ToString(), "Boolean", tag.Value.ToString())).Map(static tag => tag.Value is 1);
    public Result<byte[], DeserializationError> TryReadBytesProperty(ReadOnlySpan<char> name) => TryReadTag<ByteArrayTag>(name, "byte[]").Map(static tag => tag.Value);
    public Result<int, DeserializationError> TryReadInt32Property(ReadOnlySpan<char> name) => TryReadTag<IntTag>(name, "Int32").Map(static tag => tag.Value);
    public Result<DateTime, DeserializationError> TryReadDateTimeProperty(ReadOnlySpan<char> name) => TryReadTag<LongTag>(name, "DateTime").Map(static tag => new DateTime(tag.Value));
    public Result<Half, DeserializationError> TryReadHalfProperty(ReadOnlySpan<char> name) => TryReadTag<FloatTag>(name, "Half").Map(static tag => (Half)tag.Value);
    public Result<float, DeserializationError> TryReadSingleProperty(ReadOnlySpan<char> name) => TryReadTag<FloatTag>(name, "Single").Map(static tag => tag.Value);
    public Result<double, DeserializationError> TryReadDoubleProperty(ReadOnlySpan<char> name) => TryReadTag<DoubleTag>(name, "Double").Map(static tag => tag.Value);
    public Result<string, DeserializationError> TryReadStringProperty(ReadOnlySpan<char> name) => TryReadTag<StringTag>(name, "String").Map(static tag => tag.Value);
    public Result<T, DeserializationError> TryReadObjectProperty<T>(ReadOnlySpan<char> name) where T : IAmetrinSerializable<T>
        => TryReadTag<CompoundTag>(name, typeof(T).Name).Map(static tag => T.TryDeserialize(new AmetrinNbtReader(tag)));

    public void ReadStartObject()
    {
    }
    public void ReadEndObject()
    {
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
