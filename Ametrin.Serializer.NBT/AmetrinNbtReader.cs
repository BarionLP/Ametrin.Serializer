namespace Ametrin.Serializer.NBT;

public sealed class AmetrinNbtReader(CompoundTag tag) : IAmetrinReader
{
    private readonly CompoundTag root = tag;
    private readonly Dictionary<string, Tag>.AlternateLookup<ReadOnlySpan<char>> lookup = tag.Value.GetAlternateLookup<ReadOnlySpan<char>>();
    private Tag? currentTag = null;

    public ErrorState<DeserializationError> TryReadPropertyName(ReadOnlySpan<char> name)
    {
        if (lookup.TryGetValue(name, out var tag))
        {
            currentTag = tag;
            return default;
        }
        return DeserializationError.CreatePropertyNotFound(name.ToString());
    }

    public Result<bool, DeserializationError> TryReadBooleanValue() => TryReadTag<SbyteTag>("Boolean")
        .Require(static tag => tag.Value is 0 or 1, tag => DeserializationError.CreateInvalidValue("", "Boolean", tag.Value.ToString())).Map(static tag => tag.Value is 1);
    public Result<byte[], DeserializationError> TryReadBytesValue() => TryReadTag<ByteArrayTag>("byte[]").Map(static tag => tag.Value);
    public Result<int, DeserializationError> TryReadInt32Value() => TryReadTag<IntTag>("Int32").Map(static tag => tag.Value);
    public Result<DateTime, DeserializationError> TryReadDateTimeValue() => TryReadTag<LongTag>("DateTime").Map(static tag => new DateTime(tag.Value));
    public Result<Half, DeserializationError> TryReadHalfValue() => TryReadTag<FloatTag>("Half").Map(static tag => (Half)tag.Value);
    public Result<float, DeserializationError> TryReadSingleValue() => TryReadTag<FloatTag>("Single").Map(static tag => tag.Value);
    public Result<double, DeserializationError> TryReadDoubleValue() => TryReadTag<DoubleTag>("Double").Map(static tag => tag.Value);
    public Result<string, DeserializationError> TryReadStringValue() => TryReadTag<StringTag>("String").Map(static tag => tag.Value);

    public IAmetrinReader ReadStartObject()
    {
        if (!TryReadTag<CompoundTag>(nameof(CompoundTag)).Branch(out var tag, out _))
        {
            throw new InvalidOperationException();
        }

        return new AmetrinNbtReader(tag);
    }
    public void ReadEndObject() { }

    public void ReadStartArray() { }
    public void ReadEndArray() { }

    private Result<TTag, DeserializationError> TryReadTag<TTag>(string typeName) where TTag : Tag
    {
        if (currentTag is null) throw new InvalidOperationException();
        if (currentTag is not TTag tag)
        {
            currentTag = null;
            return DeserializationError.CreateInvalidPropertyType("", typeName);
        }

        currentTag = null;
        return tag;
    }

    public void Dispose() { }
}
