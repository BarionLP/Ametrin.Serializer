using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Ametrin.Serializer;

public readonly record struct DeserializationError(DeserializationError.Kind Type, string? PropertyName, string? ExpectedType)
{
    public enum Kind { InvalidType, PropertyNotFound, InvalidPropertyType }

    public string CreateDescription() => Type switch
    {
        Kind.InvalidType => $"Deserialization failed: Value was not of type {ExpectedType}",
        Kind.PropertyNotFound => $"Deserialization failed: Property {PropertyName} not found",
        Kind.InvalidPropertyType => $"Deserialization failed: Property {PropertyName} was not of type {ExpectedType}",
        _ => throw new UnreachableException("Unknown DeserializationError"),
    };

    [DoesNotReturn, StackTraceHidden]
    public void Throw() => throw new InvalidOperationException(CreateDescription());

    public static DeserializationError CreateInvalidType(string expectedType) => new (Kind.InvalidType, null, expectedType);
    public static DeserializationError CreatePropertyNotFound(string propertyName) => new (Kind.PropertyNotFound, propertyName, null);
    public static DeserializationError CreateInvalidPropertyType(string propertyName, string expectedType) => new (Kind.InvalidPropertyType, propertyName, expectedType);
}
