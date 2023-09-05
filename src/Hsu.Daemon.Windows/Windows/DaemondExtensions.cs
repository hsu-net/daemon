#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
#endif

using Hsu.Daemon.Cli;

using System.ServiceProcess;

namespace Hsu.Daemon.Windows;

public static class DaemondExtensions
{
    public static IDaemonServiceBuilder Configure(this IDaemond daemond, Action<HostBuilder> configure)
    {
        var builder = new HostBuilder();
        configure(builder);
        return new DaemonServiceBuilder(daemond.Code, builder.Build());
    }

    public static void Run(this IDaemonServiceBuilder builder)
    {
        var wrapper = (builder as DaemonServiceBuilder)!;
        if (wrapper.Code is not (ExitCode.Serving or ExitCode.Console)) return;

        try
        {
            Run(wrapper.Service, wrapper.Code);
        }
        catch (Exception e)
        {
            // ignored
        }
    }

    internal static void Run<T>(T service, ExitCode code, params string[] args) where T : DaemonService
    {
        switch (code)
        {
            case ExitCode.Serving:
                ServiceBase.Run(service);
                return;
            case ExitCode.Console:
                var slim = new ManualResetEventSlim(false);
                Console.CancelKeyPress += (_, _) =>
                {
                    slim.Set();
                };
                service.Start(args);
                slim.Wait();
                service.Stop();
                Console.Out.Flush();
                return;
        }
    }
}