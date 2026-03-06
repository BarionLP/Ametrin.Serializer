using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Ametrin.Serializer;

public readonly record struct DeserializationError(DeserializationError.Kind Type, string? PropertyName, string? ExpectedType, string? InvalidValue)
{
    public enum Kind { InvalidType, PropertyNotFound, InvalidPropertyType, InvalidValue }

    public string CreateDescription() => Type switch
    {
        Kind.InvalidType => $"Deserialization failed: Value was not of type {ExpectedType}",
        Kind.PropertyNotFound => $"Deserialization failed: Property {PropertyName} not found",
        Kind.InvalidPropertyType => $"Deserialization failed: Property {PropertyName} was not of type {ExpectedType}",
        Kind.InvalidValue => $"Deserialization failed: Value '{InvalidValue}' of property {PropertyName} cannot be converted to {ExpectedType}",
        _ => throw new UnreachableException("Unknown DeserializationError"),
    };

    [DoesNotReturn, StackTraceHidden]
    public void Throw() => throw new InvalidOperationException(CreateDescription());

    [DoesNotReturn, StackTraceHidden]
    public T Throw<T>() => throw new InvalidOperationException(CreateDescription());

    public static DeserializationError CreateInvalidType(string expectedType) => new (Kind.InvalidType, null, expectedType, null);
    public static DeserializationError CreatePropertyNotFound(string propertyName) => new (Kind.PropertyNotFound, propertyName, null, null);
    public static DeserializationError CreateInvalidPropertyType(string propertyName, string expectedType) => new (Kind.InvalidPropertyType, propertyName, expectedType, null);
    public static DeserializationError CreateInvalidValue(string propertyName, string expectedType, string invalidValue) => new (Kind.InvalidPropertyType, propertyName, expectedType, invalidValue);
}
