using Hsu.Daemon.Cli;
using Microsoft.Extensions.Hosting;

namespace Hsu.Daemon.Hosting;

public static class HostExtensions
{
    public static void Run(this IHost host, ExitCode code)
    {
        if (code is not (ExitCode.Serving or ExitCode.Console)) return;
        host.Run();
        if (code is ExitCode.Serving) return;
        try
        {
            host.WaitForShutdown();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public static async Task RunAsync(this IHost host, ExitCode code, CancellationToken cancellation = default)
    {
        if (code is not (ExitCode.Serving or ExitCode.Console)) return;
        await host.RunAsync(cancellation);
        if (code is ExitCode.Serving) return;
        try
        {
            await host.WaitForShutdownAsync(cancellation);
        }
        catch (Exception e)
        {
            // ignored
        }
    }
}
