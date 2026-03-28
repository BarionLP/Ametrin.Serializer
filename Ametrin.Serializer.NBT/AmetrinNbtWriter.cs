namespace Ametrin.Serializer.NBT;

public sealed class AmetrinNbtWriter(CompoundTag tag) : IAmetrinWriter
{
    private readonly CompoundTag tag = tag;
    private string? currentPropertyName;

    public void WritePropertyName(ReadOnlySpan<char> propertyName)
    {
        currentPropertyName = propertyName.ToString();
    }

    public void WriteBooleanValue(bool value)
    {
        WriteTagValue(value, static (n, v) => new SbyteTag(n, (sbyte)(v ? 1 : 0)));
    }

    public void WriteBytesValue(ReadOnlySpan<byte> value)
    {
        WriteTagValue(value, static (n, v) => new ByteArrayTag(n, [.. v]));
    }

    public void WriteDateTimeValue(DateTime value)
    {
        WriteTagValue(value.Ticks, static (n, v) => new LongTag(n, v));
    }

    public void WriteInt32Value(int value)
    {
        WriteTagValue(value, static (n, v) => new IntTag(n, v));
    }

    public void WriteHalfValue(Half value)
    {
        WriteSingleValue((float)value);
    }

    public void WriteSingleValue(float value)
    {
        WriteTagValue(value, static (n, v) => new FloatTag(n, v));
    }

    public void WriteDoubleValue(double value)
    {
        WriteTagValue(value, static (n, v) => new DoubleTag(n, v));
    }

    public void WriteStringValue(ReadOnlySpan<char> value)
    {
        WriteTagValue(value, static (n, v) => new StringTag(n, v.ToString()));
    }

    public void WriteTagValue<T>(T value, Func<string, T, Tag> factory)
        where T : allows ref struct
    {
        if (currentPropertyName is null) throw new InvalidOperationException();
        tag.Value[currentPropertyName] = factory(currentPropertyName, value);
        currentPropertyName = null;
    }

    public IAmetrinWriter WriteStartObject()
    {
        if (currentPropertyName is null) throw new InvalidOperationException();
        var newTag = new CompoundTag(currentPropertyName, []);
        tag.Value[currentPropertyName] = newTag;
        currentPropertyName = null;
        return new AmetrinNbtWriter(newTag);
    }

    public void WriteEndObject() { }

    public IAmetrinWriter WriteStartArray(int length) => throw new NotImplementedException();
    public void WriteEndArray() => throw new NotImplementedException();
}