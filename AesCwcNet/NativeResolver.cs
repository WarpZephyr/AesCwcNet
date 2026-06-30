using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AesCwcNet;

/// <summary>
/// A static class for manually resolving native imports.
/// </summary>
internal static class NativeResolver
{
    /// <summary>
    /// The name of the "runtimes" folder.
    /// </summary>
    private const string RuntimesName = "runtimes";

    /// <summary>
    /// The name of the "native" folder.
    /// </summary>
    private const string NativeName = "native";

    /// <summary>
    /// The root app folder.
    /// </summary>
    private static readonly string AppFolder = AppContext.BaseDirectory;

    /// <summary>
    /// The runtime identifier.
    /// </summary>
    private static readonly string RuntimeIdentifier = RuntimeInformation.RuntimeIdentifier;

    /// <summary>
    /// The native library folder path.
    /// </summary>
    private static readonly string NativeLibraryFolder = Path.Combine(AppFolder, RuntimesName, RuntimeIdentifier, NativeName);

    /// <summary>
    /// The prefix for native libraries.
    /// </summary>
    private static readonly string NativeLibraryPrefix;

    /// <summary>
    /// The extension for native libraries.
    /// </summary>
    private static readonly string NativeLibraryExtension;

    /// <summary>
    /// Whether or not the native resolver has been initialized.
    /// </summary>
    /// <remarks>
    /// Used as part of a trick to force static initialization.
    /// </remarks>
    private static bool IsInitialized;

    /// <summary>
    /// A static constructor for setting the import resolver.
    /// </summary>
    [SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline", Justification = "Used to check conditions for initializing multiple fields one time.")]
    static NativeResolver()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            NativeLibraryPrefix = string.Empty;
            NativeLibraryExtension = ".dll";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            NativeLibraryPrefix = "lib";
            NativeLibraryExtension = ".dylib";
        }
        else
        {
            // Assume linux
            NativeLibraryPrefix = "lib";
            NativeLibraryExtension = ".so";
        }

        NativeLibrary.SetDllImportResolver(typeof(NativeResolver).Assembly, ResolveDll);
    }

    /// <summary>
    /// Initializes the resolver.
    /// </summary>
    public static void Initialize()
        => IsInitialized = true;

    /// <summary>
    /// A native library resolver callback.
    /// </summary>
    /// <param name="libraryName">The name of the native library.</param>
    /// <param name="assembly">The assembly to search.</param>
    /// <param name="searchPath">The search path.</param>
    /// <returns>A pointer to the native library or <see cref="IntPtr.Zero"/> if not found.</returns>
    private static IntPtr ResolveDll(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName == "aescwc")
        {
            if (NativeLibrary.TryLoad(libraryName, assembly, searchPath, out nint handle))
            {
                return handle;
            }

            if (NativeLibrary.TryLoad(GetLibraryPath(libraryName), out handle))
            {
                return handle;
            }
        }

        return IntPtr.Zero;
    }

    /// <summary>
    /// Gets the full file path of the specified library.
    /// </summary>
    /// <param name="name">The name of the library.</param>
    /// <returns>The full file path of the specified library.</returns>
    private static string GetLibraryPath(string libraryName)
        => Path.Combine(NativeLibraryFolder, $"{NativeLibraryPrefix}{libraryName}{NativeLibraryExtension}");
}
