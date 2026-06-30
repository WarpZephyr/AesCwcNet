using System;
using System.Runtime.InteropServices;
using static AesCwcNet.CwcNative;

namespace AesCwcNet;

/// <summary>
/// An object representing decryption and encryption for AES CWC mode.
/// </summary>
public class Cwc : IDisposable
{
    /// <summary>
    /// The underlying native cwc context.
    /// </summary>
    private unsafe void* _context;

    /// <summary>
    /// The size in bytes of the underlying native cwc context.
    /// </summary>
    private readonly int _contextSize;

    /// <summary>
    /// Whether or not this object has been disposed.
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// Create a new <see cref="Cwc"/> with the specified key.
    /// </summary>
    /// <param name="key">The key to use.</param>
    public unsafe Cwc(ReadOnlySpan<byte> key)
    {
        _contextSize = cwc_get_ctx_size();
        _context = NativeMemory.AlignedAlloc((nuint)_contextSize, (nuint)cwc_get_ctx_align());

        fixed (byte* pKey = key)
        {
            int result = cwc_init_and_key(pKey, (uint)key.Length, _context);
            CwcGuard.Check(result, $"Native Cwc Context initialization failed; Status: {result}");
        }
    }

    /// <summary>
    /// Encrypts an entire message.
    /// </summary>
    /// <param name="iv">The initialization vector.</param>
    /// <param name="header">The header buffer.</param>
    /// <param name="message">The message buffer.</param>
    /// <param name="tag">The tag buffer.</param>
    public unsafe void Encrypt(ReadOnlySpan<byte> iv, ReadOnlySpan<byte> header, ReadOnlySpan<byte> message, Span<byte> tag)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        fixed (byte* pIv = iv)
        fixed (byte* pHdr = header)
        fixed (byte* pMsg = message)
        fixed (byte* pTag = tag)
        {
            int result = cwc_encrypt_message(pIv, (uint)iv.Length, pHdr, (uint)header.Length, pMsg, (uint)message.Length, pTag, (uint)tag.Length, _context);
            CwcGuard.Check(result, $"Native Cwc encryption failed; Status: {result}");
        }
    }

    /// <summary>
    /// Encrypts an entire message.
    /// </summary>
    /// <param name="pIv">A pointer to the initialization vector.</param>
    /// <param name="ivLength">The iv length in bytes.</param>
    /// <param name="pHeader">A pointer to the header buffer.</param>
    /// <param name="headerLength">The header length in bytes.</param>
    /// <param name="pMessage">A pointer to the message buffer.</param>
    /// <param name="messageLength">The message length in bytes.</param>
    /// <param name="pTag">A pointer to the tag buffer.</param>
    /// <param name="tagLength">The tag length in bytes.</param>
    public unsafe void Encrypt(byte* pIv, uint ivLength, byte* pHeader, uint headerLength, byte* pMessage, uint messageLength, byte* pTag, uint tagLength)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        int result = cwc_encrypt_message(pIv, ivLength, pHeader, headerLength, pMessage, messageLength, pTag, tagLength, _context);
        CwcGuard.Check(result, $"Native Cwc encryption failed; Status: {result}");
    }

    /// <summary>
    /// Decrypts an entire message.
    /// </summary>
    /// <param name="iv">The initialization vector.</param>
    /// <param name="header">The header buffer.</param>
    /// <param name="message">The message buffer.</param>
    /// <param name="tag">The tag buffer.</param>
    public unsafe void Decrypt(ReadOnlySpan<byte> iv, ReadOnlySpan<byte> header, ReadOnlySpan<byte> message, Span<byte> tag)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        fixed (byte* pIv = iv)
        fixed (byte* pHdr = header)
        fixed (byte* pMsg = message)
        fixed (byte* pTag = tag)
        {
            int result = cwc_decrypt_message(pIv, (uint)iv.Length, pHdr, (uint)header.Length, pMsg, (uint)message.Length, pTag, (uint)tag.Length, _context);
            CwcGuard.Check(result, $"Native Cwc decryption failed; Status: {result}");
        }
    }

    /// <summary>
    /// Decrypts an entire message.
    /// </summary>
    /// <param name="pIv">A pointer to the initialization vector.</param>
    /// <param name="ivLength">The iv length in bytes.</param>
    /// <param name="pHeader">A pointer to the header buffer.</param>
    /// <param name="headerLength">The header length in bytes.</param>
    /// <param name="pMessage">A pointer to the message buffer.</param>
    /// <param name="messageLength">The message length in bytes.</param>
    /// <param name="pTag">A pointer to the tag buffer.</param>
    /// <param name="tagLength">The tag length in bytes.</param>
    public unsafe void Decrypt(byte* pIv, uint ivLength, byte* pHeader, uint headerLength, byte* pMessage, uint messageLength, byte* pTag, uint tagLength)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        int result = cwc_decrypt_message(pIv, ivLength, pHeader, headerLength, pMessage, messageLength, pTag, tagLength, _context);
        CwcGuard.Check(result, $"Native Cwc decryption failed; Status: {result}");
    }

    /// <summary>
    /// Tries to encrypt an entire message.
    /// </summary>
    /// <param name="iv">The initialization vector.</param>
    /// <param name="header">The header buffer.</param>
    /// <param name="message">The message buffer.</param>
    /// <param name="tag">The tag buffer.</param>
    /// <returns>Whether or not the encryption succeeded.</returns>
    public unsafe bool TryEncrypt(ReadOnlySpan<byte> iv, ReadOnlySpan<byte> header, ReadOnlySpan<byte> message, Span<byte> tag)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        fixed (byte* pIv = iv)
        fixed (byte* pHdr = header)
        fixed (byte* pMsg = message)
        fixed (byte* pTag = tag)
        {
            return cwc_encrypt_message(pIv, (uint)iv.Length, pHdr, (uint)header.Length, pMsg, (uint)message.Length, pTag, (uint)tag.Length, _context) == RETURN_GOOD;
        }
    }

    /// <summary>
    /// Tries to encrypt an entire message.
    /// </summary>
    /// <param name="pIv">A pointer to the initialization vector.</param>
    /// <param name="ivLength">The iv length in bytes.</param>
    /// <param name="pHeader">A pointer to the header buffer.</param>
    /// <param name="headerLength">The header length in bytes.</param>
    /// <param name="pMessage">A pointer to the message buffer.</param>
    /// <param name="messageLength">The message length in bytes.</param>
    /// <param name="pTag">A pointer to the tag buffer.</param>
    /// <param name="tagLength">The tag length in bytes.</param>
    /// <returns>Whether or not the encryption succeeded.</returns>
    public unsafe bool TryEncrypt(byte* pIv, uint ivLength, byte* pHeader, uint headerLength, byte* pMessage, uint messageLength, byte* pTag, uint tagLength)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return cwc_encrypt_message(pIv, ivLength, pHeader, headerLength, pMessage, messageLength, pTag, tagLength, _context) == RETURN_GOOD;
    }

    /// <summary>
    /// Tries to decrypt an entire message.
    /// </summary>
    /// <param name="iv">The initialization vector.</param>
    /// <param name="header">The header buffer.</param>
    /// <param name="message">The message buffer.</param>
    /// <param name="tag">The tag buffer.</param>
    /// <returns>Whether or not the decryption succeeded.</returns>
    public unsafe bool TryDecrypt(ReadOnlySpan<byte> iv, ReadOnlySpan<byte> header, ReadOnlySpan<byte> message, Span<byte> tag)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        fixed (byte* pIv = iv)
        fixed (byte* pHdr = header)
        fixed (byte* pMsg = message)
        fixed (byte* pTag = tag)
        {
            return cwc_decrypt_message(pIv, (uint)iv.Length, pHdr, (uint)header.Length, pMsg, (uint)message.Length, pTag, (uint)tag.Length, _context) == RETURN_GOOD;
        }
    }

    /// <summary>
    /// Tries to decrypt an entire message.
    /// </summary>
    /// <param name="pIv">A pointer to the initialization vector.</param>
    /// <param name="ivLength">The iv length in bytes.</param>
    /// <param name="pHeader">A pointer to the header buffer.</param>
    /// <param name="headerLength">The header length in bytes.</param>
    /// <param name="pMessage">A pointer to the message buffer.</param>
    /// <param name="messageLength">The message length in bytes.</param>
    /// <param name="pTag">A pointer to the tag buffer.</param>
    /// <param name="tagLength">The tag length in bytes.</param>
    /// <returns>Whether or not the decryption succeeded.</returns>
    public unsafe bool TryDecrypt(byte* pIv, uint ivLength, byte* pHeader, uint headerLength, byte* pMessage, uint messageLength, byte* pTag, uint tagLength)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return cwc_decrypt_message(pIv, ivLength, pHeader, headerLength, pMessage, messageLength, pTag, tagLength, _context) == RETURN_GOOD;
    }

    #region IDisposable

    protected virtual unsafe void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (_context != null)
            {
                _ = cwc_end(_context);
                NativeMemory.Clear(_context, (nuint)_contextSize);
                NativeMemory.AlignedFree(_context);
                _context = null;
            }

            _disposed = true;
        }
    }

    ~Cwc()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
