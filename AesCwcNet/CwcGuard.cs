using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using static AesCwcNet.CwcNative;

namespace AesCwcNet;

/// <summary>
/// A guard helper for checking native call results.
/// </summary>
internal static class CwcGuard
{
    /// <summary>
    /// Check the specified result, throwing if not <see cref="RETURN_GOOD"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="error">The error message.</param>
    /// <exception cref="CryptographicException">The result was not <see cref="RETURN_GOOD"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check(int result, string error)
    {
        if (result != RETURN_GOOD)
        {
            throw new CryptographicException(error);
        }
    }

    /// <summary>
    /// Check the specified result, throwing if not <see cref="RETURN_GOOD"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="error">The interpolated error message.</param>
    /// <exception cref="CryptographicException">The result was not <see cref="RETURN_GOOD"/>.</exception>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check(int result, [InterpolatedStringHandlerArgument(nameof(result))] ref CwcErrorStringHandler error)
    {
        if (result != RETURN_GOOD)
        {
            throw new CryptographicException(error.ToStringAndClear());
        }
    }
}
