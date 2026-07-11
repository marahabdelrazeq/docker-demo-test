using System.Runtime.InteropServices;

namespace CommonLibrary.Utilities
{
    /// <summary>
    /// Provides utility methods for identifying the current operating system.
    /// </summary>
    public static class OperatingSystemHelper
    {
        /// <summary>
        /// Determines if the current OS is Windows.
        /// </summary>
        public static bool IsWindows() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// Determines if the current OS is macOS.
        /// </summary>
        public static bool IsMacOS() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        /// <summary>
        /// Determines if the current OS is Linux.
        /// </summary>
        public static bool IsLinux() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
}
