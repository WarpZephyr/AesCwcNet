using System.Runtime.CompilerServices;

namespace AesCwcNet;

/// <summary>
/// An interpolated string handler for error messages.
/// </summary>
/// <remarks>
/// This allows for interpolated strings that do not get built unless they are needed.
/// </remarks>
[InterpolatedStringHandler]
internal ref struct CwcErrorStringHandler
{
    /// <summary>
    /// The underlying handler.
    /// </summary>
    private DefaultInterpolatedStringHandler _handler;

    /// <summary>
    /// Creates a new <see cref="CwcErrorStringHandler"/>.
    /// </summary>
    /// <param name="literalLength">The number of constant characters outside of interpolation expressions in the interpolated string.</param>
    /// <param name="formattedCount">The number of interpolation expressions in the interpolated string.</param>
    /// <param name="result">The result to check.</param>
    /// <param name="isEnabled">Whether or not the handler should be built at all.</param>
    public CwcErrorStringHandler(int literalLength, int formattedCount, int result, out bool isEnabled)
    {
        isEnabled = result != CwcNative.RETURN_GOOD;
        if (isEnabled)
        {
            _handler = new DefaultInterpolatedStringHandler(literalLength, formattedCount);
        }
        else
        {
            _handler = default;
        }
    }

    /// <summary>
    /// Writes the specified string to the handler.
    /// </summary>
    /// <param name="value">The string to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string value)
        => _handler.AppendLiteral(value);

    /// <summary>
    /// Writes the specified value to the handler.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value)
        => _handler.AppendFormatted(value);

    /// <summary>
    /// Gets the built <see cref="string"/> and clears the handler.
    /// </summary>
    /// <remarks>
    /// This releases any resources used by the handler.<br/>
    /// The method should be invoked only once and as the last thing performed on the handler.<br/>
    /// Subsequent use is erroneous, ill-defined, and may destabilize the process.<br/>
    /// Using any other copies of the handler after <see cref="ToStringAndClear" /> is called on any one of them is also erroneous.
    /// </remarks>
    /// <returns>The built string.</returns>
    internal string ToStringAndClear()
        => _handler.ToStringAndClear();
}
