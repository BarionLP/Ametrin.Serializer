namespace Ametrin.Serializer.NBT;

public sealed class AmetrinNbtWriter(CompoundTag tag) : IAmetrinWriter
{
    private readonly CompoundTag root = tag;
    private string? currentPropertyName;
    private readonly Stack<CompoundTag> stack = [];
    private CompoundTag currentTag = tag;

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

    public void WriteTagValue<T>(T value, Func<string, T, Tag> tag)
        where T : allows ref struct
    {
        if (currentPropertyName is null) throw new InvalidOperationException();
        currentTag.Value[currentPropertyName] = tag(currentPropertyName, value);
        currentPropertyName = null;
    }

    public void WriteStartObject()
    {
        if (currentPropertyName is null) throw new InvalidOperationException();
        stack.Push(currentTag);
        currentTag = new(currentPropertyName, []);
        currentPropertyName = null;
    }

    public void WriteEndObject()
    {
        if (!stack.TryPop(out var cur))
        {
            throw new InvalidOperationException();
        }
        currentTag = cur;
    }

    public void WriteStartArray(int length) => throw new NotImplementedException();
    public void WriteEndArray() => throw new NotImplementedException();
}