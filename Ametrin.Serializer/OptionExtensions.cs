using System.Diagnostics.CodeAnalysis;
using Ametrin.Optional;

namespace Ametrin.Serializer;

public static class OptionExtensions
{
    // testing the pattern for Ametrin.Optional
    public static bool Branch<T, E>(this Result<T, E> result, [MaybeNullWhen(false)] out T value, [MaybeNullWhen(true)] out E error)
    {
        if (OptionsMarshall.TryGetError(result, out error))
        {
            value = default;
            return false;
        }

        error = default;
        value = result.OrThrow();
        return true;
    }
}
