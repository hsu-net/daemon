using System.Diagnostics;
// ReSharper disable MemberCanBePrivate.Global

namespace Hsu.Daemon;

internal static class ProcessHelper
{
    public static int Command(string bin, string args, out string output)
    {
        #if DEBUG
        Debug.WriteLine($"{bin} {args}");
        #endif
        using var process = Process.Start(new ProcessStartInfo()
        {
            FileName = bin,
            Arguments = args,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            // RedirectStandardError = true
        })!;

        output = process.StandardOutput.ReadToEnd().Trim();
        
        #if DEBUG
        Debug.WriteLine($"{bin} {args} > {output}");
        #endif
        process.WaitForExit();
        return process.ExitCode;
    }

    public static int Command(string bin, string args) => Command(bin, args, out _);
}
