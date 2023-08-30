using Hsu.Daemon.Cli;
#if WEB
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.AspNetCore.Hosting;
using IHost = Microsoft.AspNetCore.Hosting.IWebHost;
#else
using Microsoft.Extensions.Hosting;
using IHost = Microsoft.Extensions.Hosting.IHost;
#endif

namespace Hsu.Daemon.Hosting;

public static class HostExtensions
{
    public static void Run(this IHost host, ExitCode code)
    {
        if (code is not (ExitCode.Serving or ExitCode.Console)) return;
        
        try
        {
            #if WEB
            if (code is ExitCode.Serving)
            {
                host.RunAsService();
                return;
            }
            #endif
        
            host.Run();
        }
        catch (Exception e)
        {
            // ignored
        }
    }

    public static async Task RunAsync(this IHost host, ExitCode code, CancellationToken cancellation = default)
    {
        if (code is not (ExitCode.Serving or ExitCode.Console)) return;

        try
        {
            #if WEB
            if (code is ExitCode.Serving)
            {
                await Task.Yield();
                host.RunAsService();
                return;
            }
            #endif
        
            await host.RunAsync(cancellation);
        }
        catch (Exception e)
        {
            // ignored
        }
    }
}