// ReSharper disable UnusedMember.Global
namespace Hsu.Daemon;

internal static class OsHelper
{
    public static bool IsWindows()
    {
#if NET5_0_OR_GREATER
        return OperatingSystem.IsWindows();
#else
        return Environment.OSVersion.Platform is PlatformID.Win32NT or PlatformID.Win32Windows or PlatformID.Win32S;
#endif
    }

    public static bool IsLinux()
    {
        #if NET5_0_OR_GREATER
        return OperatingSystem.IsLinux();
        #else
        return Environment.OSVersion.Platform is PlatformID.Win32NT or PlatformID.Win32Windows or PlatformID.Win32S;
        #endif
    }

    public static bool IsMac()
    {
        #if NET5_0_OR_GREATER
        return OperatingSystem.IsMacOS();
        #else
        return Environment.OSVersion.Platform is PlatformID.MacOSX;
        #endif
    }

    public static string? GetDotNet()
    {
        var dotnet = Environment.GetEnvironmentVariable("DOTNET_ROOT");
        if (!dotnet.IsNullOrWhiteSpace()) return dotnet;

        ProcessHelper.Command("which", "dotnet", out var output);
        if (!output.IsNullOrWhiteSpace()) return output.Trim();

        return null;
    }
}