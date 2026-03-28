using System.IO.Compression;
using System.Text.Json;
using Ametrin.Optional;
using Ametrin.Serializer.Converters;
using BenchmarkDotNet.Attributes;

namespace Ametrin.Serializer.Sample;

[MemoryDiagnoser(true)]
public class Benchmarks
{
    private AmetrinSerializationOptions OptionsNone = new();

    private AmetrinSerializationOptions OptionsCompression = new()
    {
        CompressionLevel = CompressionLevel.Optimal,
    };

    private AmetrinSerializationOptions OptionsEncrption = new()
    {
        Encryption = new()
        {
            Password = "Am3tr!n"
        },
    };

    private AmetrinSerializationOptions OptionsBoth = new()
    {
        CompressionLevel = CompressionLevel.Optimal,
        Encryption = new()
        {
            Password = "Am3tr!n"
        },
    };

    private readonly BenchmarkData data = new()
    {

    };

    private readonly MemoryStream stream = new();

    [IterationSetup]
    public void IterationSetup()
    {
        stream.Seek(0, SeekOrigin.Begin);
    }

    [Benchmark(Baseline = true)]
    public void System_Text_Json()
    {
        JsonSerializer.Serialize(stream, data);
    }

    [Benchmark]
    public void Ametrin_None()
    {
        AmetrinSerializer.Serialize(stream, data);
    }

    [Benchmark]
    public void Ametrin_Compressed()
    {
        AmetrinSerializer.Serialize(stream, data, OptionsCompression);
    }

    [Benchmark]
    public void Ametrin_Encypted()
    {
        AmetrinSerializer.Serialize(stream, data, OptionsEncrption);
    }

    [Benchmark]
    public void Ametrin_Both()
    {
        AmetrinSerializer.Serialize(stream, data, OptionsBoth);
    }
}

[GeneratedConverter(AllProperties: true)]
internal sealed partial class BenchmarkData() : ISerializationConverter<BenchmarkData>
{
    private float pi { get; } = float.Pi;
    private double pi2 { get; } = double.Pi;
    private string lorem { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private int value { get; } = int.MaxValue;
    private int value2 { get; } = int.MinValue;
    public Test t1 { get; } = Test.c;
    [Serialize(Converter: typeof(EnumConverter<Test>))] public Test t2 = Test.v;
    private string lorem1 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem2 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem3 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem4 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem5 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem6 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem7 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem8 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem9 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem10 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem11 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem12 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem13 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem14 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem15 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem16 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem17 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem18 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";
    private string lorem19 { get; } = "lorem ipsum, with a lot more text cause stresstesting yk";

    public BenchmarkData(float pi, double pi2, string lorem, int value, int value2, Test t1, Test t2, string lorem1, string lorem2, string lorem3, string lorem4, string lorem5, string lorem6, string lorem7, string lorem8, string lorem9, string lorem10, string lorem11, string lorem12, string lorem13, string lorem14, string lorem15, string lorem16, string lorem17, string lorem18, string lorem19) : this()
    {
        this.pi = pi;
        this.pi2 = pi2;
        this.lorem = lorem;
        this.value = value;
        this.value2 = value2;
        this.t1 = t1;
        this.t2 = t2;
        this.lorem1 = lorem1;
        this.lorem2 = lorem2;
        this.lorem3 = lorem3;
        this.lorem4 = lorem4;
        this.lorem5 = lorem5;
        this.lorem6 = lorem6;
        this.lorem7 = lorem7;
        this.lorem8 = lorem8;
        this.lorem9 = lorem9;
        this.lorem10 = lorem10;
        this.lorem11 = lorem11;
        this.lorem12 = lorem12;
        this.lorem13 = lorem13;
        this.lorem14 = lorem14;
        this.lorem15 = lorem15;
        this.lorem16 = lorem16;
        this.lorem17 = lorem17;
        this.lorem18 = lorem18;
        this.lorem19 = lorem19;
    }


    public enum Test { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z }
}
