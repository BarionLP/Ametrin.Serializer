using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using Ametrin.Optional;
using Ametrin.Serializer.Converters;
using Ametrin.Serializer.Readers;
using Ametrin.Serializer.Writers;

namespace Ametrin.Serializer;

public static class AmetrinSerializer
{
    public static AmetrinSerializationOptions DefaultOptions { get; } = new AmetrinSerializationOptions
    {

    };

    public static AmetrinJsonWriter CreateJsonSerializer<T>(Stream output, AmetrinSerializationOptions? options = null) where T : ISerializationConverter<T>
    {
        options ??= DefaultOptions;

        using var encryptionStream = options.Encryption is null ? null : EncryptStream(output, options.Encryption);
        using var compressionStream = options.CompressionLevel is CompressionLevel.NoCompression ? null : CompressStream(encryptionStream ?? output, options.CompressionLevel);

        return new AmetrinJsonWriter(compressionStream ?? encryptionStream ?? output, leaveOpen: true);
        // using var writer = new AmetrinBinaryWriter(compressionStream ?? encryptionStream ?? output, leaveOpen: true);

    }

    public static void WriteDynamic<T>(IAmetrinWriter writer, T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (!knownWriters.TryGetValue(value.GetType(), out var write))
        {
            throw new InvalidOperationException($"Unkown type {value.GetType().Name}");
        }

        using var dynamicWriter = writer.WriteStartObject();
        dynamicWriter.WriteStringProperty("$type", GetFriendlyName(value.GetType()));
        dynamicWriter.WritePropertyName("$value");
        write(dynamicWriter, value);
        writer.WriteEndObject();
    }

    public static Result<T, DeserializationError> TryReadDynamic<T>(IAmetrinReader reader)
    {
        using var dynamicReader = reader.ReadStartObject();
        var type = dynamicReader.ReadStringProperty("$type");

        if (!namedReaders.TryGetValue(type, out var read))
        {
            throw new InvalidOperationException($"Unkown type {type}");
        }
        dynamicReader.ReadPropertyName("$value");
        var value = read(dynamicReader).Map(static o => (T)o);
        reader.ReadEndObject();
        return value;
    }

    public static Result<T, DeserializationError> TryDeserialize<T>(Stream input, AmetrinSerializationOptions? options = null)
    {
        options ??= DefaultOptions;

        using var decryptionStream = options.Encryption is null ? null : DecryptStream(input, options.Encryption);
        using var decompressionStream = options.CompressionLevel is CompressionLevel.NoCompression ? null : DecompressStream(decryptionStream ?? input);

        var reader = AmetrinJsonReader.Create(decompressionStream ?? decryptionStream ?? input);
        // using var reader = new AmetrinBinaryReader(decompressionStream ?? decryptionStream ?? input, leaveOpen: true);

        if (knownReaders.TryGetValue(typeof(T), out var supplier))
        {
            return supplier(reader).Map(static o => (T)o);
        }

        throw new InvalidOperationException($"Unkown type {typeof(T).Name}");
    }

    public static Stream EncryptStream(Stream stream, AmetrinSerializationOptions.EncryptionOptions options)
    {
        Span<byte> salt = stackalloc byte[options.SaltSize];
        RandomNumberGenerator.Fill(salt);

        using var aes = Aes.Create();
        Rfc2898DeriveBytes.Pbkdf2(options.Password, salt, destination: aes.Key, options.Iterations, options.HashAlgorithm);
        aes.GenerateIV();

        stream.Write(salt);
        stream.Write(aes.IV);

        return new CryptoStream(stream, aes.CreateEncryptor(), CryptoStreamMode.Write, leaveOpen: true);
    }

    public static Stream DecryptStream(Stream stream, AmetrinSerializationOptions.EncryptionOptions options)
    {
        Span<byte> salt = stackalloc byte[options.SaltSize];
        stream.ReadExactly(salt);

        using var aes = Aes.Create();
        stream.ReadExactly(aes.IV);
        Rfc2898DeriveBytes.Pbkdf2(options.Password, salt, destination: aes.Key, options.Iterations, options.HashAlgorithm);

        return new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Read, leaveOpen: true);
    }

    public static Stream CompressStream(Stream stream, CompressionLevel compressionLevel)
    {
        return new ZLibStream(stream, compressionLevel, leaveOpen: true);
    }

    public static Stream DecompressStream(Stream stream)
    {
        return new ZLibStream(stream, CompressionMode.Decompress, leaveOpen: true);
    }

    private static readonly Dictionary<string, Func<IAmetrinReader, Result<object, DeserializationError>>> namedReaders = [];
    private static readonly Dictionary<Type, Func<IAmetrinReader, Result<object, DeserializationError>>> knownReaders = [];
    private static readonly Dictionary<Type, Action<IAmetrinWriter, object>> knownWriters = [];

    public static void RegisterSerializer<T>()
        where T : ISerializationConverter<T>
    {
        RegisterSerializer<T, T>();
    }

    public static void RegisterSerializer<TConverter, TValue>()
        where TConverter : ISerializationConverter<TValue>
    {
        var name = GetFriendlyName(typeof(TValue));
        Debug.Assert(!namedReaders.ContainsKey(name));
        namedReaders[name] = static reader => TConverter.TryReadValue(reader).As<object>();
        knownReaders[typeof(TValue)] = static reader => TConverter.TryReadValue(reader).As<object>();
        knownWriters[typeof(TValue)] = static (writer, value) => TConverter.WriteValue(writer, (TValue)value);
    }

    public static void RegisterNamedReader<TConverter, TValue>(string name)
        where TConverter : ISerializationConverter<TValue>
    {
        namedReaders[name] = static reader => TConverter.TryReadValue(reader).As<object>();
    }

    public static string GetFriendlyName(Type type)
    {
        if (!type.IsGenericType)
            return type.Name;

        var name = type.GetGenericTypeDefinition().FullName!;
        name = name![..name.IndexOf('`')];
        var args = type.GetGenericArguments().Select(GetFriendlyName);

        return $"{name}<{string.Join(", ", args)}>";
    }
}

[GenerateSerializer]
public sealed partial class AmetrinSerializationOptions
{
    [Serialize(Converter: typeof(EnumConverter<CompressionLevel>))] public CompressionLevel CompressionLevel { get; init; } = CompressionLevel.NoCompression;
    [Serialize] public EncryptionOptions? Encryption { get; init; } = null;

    [GenerateSerializer]
    public sealed partial class EncryptionOptions
    {
        [Serialize] public required string Password { get; init; }
        [Serialize] public int SaltSize { get; init; } = 12;
        [Serialize] public int Iterations { get; init; } = 10_000;
        [Serialize(Converter: typeof(HashAlgorithmConverter))] public HashAlgorithmName HashAlgorithm { get; init; } = HashAlgorithmName.SHA3_256;
    }
}

#pragma warning disable CS9113 // Parameter is unread.

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class GenerateSerializerAttribute(bool SerializeTypeName = false, string? TypeName = null, bool AllProperties = false, bool AllFields = false) : Attribute;

[AttributeUsage(AttributeTargets.Class)]
public sealed class GeneratedConverterAttribute(string? TypeName = null, bool AllProperties = false, bool AllFields = false) : Attribute;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class SerializeAttribute(Type? Converter = null) : Attribute;
