namespace Ametrin.Serializer.NBT;

public sealed class AmetrinNbtWriter(CompoundTag tag) : IAmetrinWriter
{
    private readonly CompoundTag tag = tag;

    public void WriteBooleanProperty(ReadOnlySpan<char> properyName, bool value)
    {
        var name = properyName.ToString();
        tag.Value[name] = new SbyteTag(name, (sbyte)(value ? 1 : 0));
    }

    public void WriteBytesProperty(ReadOnlySpan<char> properyName, ReadOnlySpan<byte> value)
    {
        var name = properyName.ToString();
        tag.Value[name] = new ByteArrayTag(name, [.. value]);
    }

    public void WriteDateTimeProperty(ReadOnlySpan<char> properyName, DateTime value)
    {
        var name = properyName.ToString();
        tag.Value[name] = new LongTag(name, value.Ticks);
    }

    public void WriteInt32Property(ReadOnlySpan<char> properyName, int value)
    {
        var name = properyName.ToString();
        tag.Value[name] = new IntTag(name, value);
    }

    public void WriteHalfProperty(ReadOnlySpan<char> properyName, Half value)
    {
        var name = properyName.ToString();
        tag.Value[name] = new FloatTag(name, (float)value);
    }

    public void WriteSingleProperty(ReadOnlySpan<char> properyName, float value)
    {
        var name = properyName.ToString();
        tag.Value[name] = new FloatTag(name, value);
    }

    public void WriteDoubleProperty(ReadOnlySpan<char> properyName, double value)
    {
        var name = properyName.ToString();
        tag.Value[name] = new DoubleTag(name, value);
    }

    public void WriteStringProperty(ReadOnlySpan<char> properyName, ReadOnlySpan<char> value)
    {
        var name = properyName.ToString();
        tag.Value[name] = new StringTag(name, value.ToString());
    }

    public void WriteObjectProperty<T>(ReadOnlySpan<char> properyName, T value) where T : IAmetrinSerializable<T>
    {
        var name = properyName.ToString();
        var writer = new AmetrinNbtWriter(new(name, []));
        T.Serialize(value, writer);
        tag.Value[name] = writer.tag;
    }

    public void WriteStartObject() { }
    public void WriteEndObject() { }
}