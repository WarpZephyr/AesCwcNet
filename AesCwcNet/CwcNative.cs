using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AesCwcNet;

/// <summary>
/// A class for native interop with aes_cwc.
/// </summary>
[SuppressMessage("Security", "CA5393:Do not use unsafe DllImportSearchPath value",
    Justification = "Required to locate the aes_cwc native binaries portably.")]
internal static unsafe partial class CwcNative
{
    /// <summary>
    /// The library name.
    /// </summary>
    private const string LIBNAME = "aescwc";

    /// <summary>
    /// The value that is returned on success. 
    /// </summary>
    public const int RETURN_GOOD = 0;

    /// <summary>
    /// The value that is returned on warning.
    /// </summary>
    public const int RETURN_WARN = 1;

    /// <summary>
    /// The value that is returned on error.
    /// </summary>
    public const int RETURN_ERROR = -1;

    /// <summary>
    /// Ensures the native resolver is initialized.
    /// </summary>
    static CwcNative()
    {
        NativeResolver.Initialize();
    }

    /// <summary>
    /// Initialize mode and set key.
    /// </summary>
    /// <param name="key">The key value.</param>
    /// <param name="key_len">The key length in bytes.</param>
    /// <param name="ctx">The mode context.</param>
    [LibraryImport(LIBNAME)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
    public static partial int cwc_init_and_key(byte* key, uint key_len, void* ctx);

    /// <summary>
    /// Clean up and end operation.
    /// </summary>
    /// <param name="ctx">The mode context.</param>
    [LibraryImport(LIBNAME)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
    public static partial int cwc_end(void* ctx);

    /// <summary>
    /// Encrypt an entire message.
    /// </summary>
    /// <param name="iv">The initialisation vector.</param>
    /// <param name="iv_len">The iv length in bytes.</param>
    /// <param name="hdr">The header buffer</param>
    /// <param name="hdr_len">The hdr length in bytes.</param>
    /// <param name="msg">The message buffer.</param>
    /// <param name="msg_len">The msg length in bytes.</param>
    /// <param name="tag">The buffer for the tag.</param>
    /// <param name="tag_len">The tag length in bytes.</param>
    /// <param name="ctx">The mode context.</param>
    [LibraryImport(LIBNAME)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
    public static partial int cwc_encrypt_message(
        byte* iv, uint iv_len,
        byte* hdr, uint hdr_len,
        byte* msg, uint msg_len,
        byte* tag, uint tag_len,
        void* ctx
    );

    /// <summary>
    /// Decrypt an entire message.
    /// </summary>
    /// <param name="iv">The initialisation vector.</param>
    /// <param name="iv_len">The iv length in bytes.</param>
    /// <param name="hdr">The header buffer</param>
    /// <param name="hdr_len">The hdr length in bytes.</param>
    /// <param name="msg">The message buffer.</param>
    /// <param name="msg_len">The msg length in bytes.</param>
    /// <param name="tag">The buffer for the tag.</param>
    /// <param name="tag_len">The tag length in bytes.</param>
    /// <param name="ctx">The mode context.</param>
    /// <returns>RETURN_GOOD is returned if the input tag matches that for the decrypted message.</returns>
    [LibraryImport(LIBNAME)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
    public static partial int cwc_decrypt_message(
        byte* iv, uint iv_len,
        byte* hdr, uint hdr_len,
        byte* msg, uint msg_len,
        byte* tag, uint tag_len,
        void* ctx
    );

    /// <summary>
    /// Gets the cwc context size.
    /// </summary>
    /// <returns>The cwc context size.</returns>
    [LibraryImport(LIBNAME)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
    public static partial int cwc_get_ctx_size();

    /// <summary>
    /// Gets the cwc context alignment.
    /// </summary>
    /// <returns>The cwc context alignment.</returns>
    [LibraryImport(LIBNAME)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
    public static partial int cwc_get_ctx_align();
}
