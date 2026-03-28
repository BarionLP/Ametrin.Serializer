namespace Ametrin.Serializer.NBT;

public abstract class AmetrinNbtReader : IAmetrinReader
{
    public abstract ErrorState<DeserializationError> TryReadPropertyName(ReadOnlySpan<char> name);

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

        return new AmetrinNbtObjectReader(tag);
    }
    public void ReadEndObject() { }

    public IAmetrinReader ReadStartArray(out int itemCount) => throw new NotImplementedException();
    public void ReadEndArray() { }

    protected abstract Tag ConsumeCurrentTag();

    private Result<TTag, DeserializationError> TryReadTag<TTag>(string typeName) where TTag : Tag
    {
        return ConsumeCurrentTag() is TTag tag ? tag : DeserializationError.CreateInvalidPropertyType("", typeName);
    }

    public void Dispose() { }
}

public sealed class AmetrinNbtObjectReader(CompoundTag root) : AmetrinNbtReader
{
    private readonly CompoundTag root = root;
    private readonly Dictionary<string, Tag>.AlternateLookup<ReadOnlySpan<char>> lookup = root.Value.GetAlternateLookup<ReadOnlySpan<char>>();
    private Tag? currentTag = null;

    public override ErrorState<DeserializationError> TryReadPropertyName(ReadOnlySpan<char> name)
    {
        if (lookup.TryGetValue(name, out var tag))
        {
            currentTag = tag;
            return default;
        }
        return DeserializationError.CreatePropertyNotFound(name.ToString());
    }

    protected override Tag ConsumeCurrentTag()
    {
        if (currentTag is { } tag)
        {
            currentTag = null;
            return tag;
        }

        throw new InvalidOperationException();
    }
}