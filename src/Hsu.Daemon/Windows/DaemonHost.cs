#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
#endif
using System.ServiceProcess;
using Hsu.Daemon.Cli;

namespace Hsu.Daemon.Windows;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("windows")]
#endif
public static class DaemonHost
{
    public static void Run(Action<DaemonBuilder> configure, string[] args)
    {
        var builder = new DaemonBuilder();
        configure(builder);
        builder.Run(args);
    }

    public static void Run<T>(params string[] args) where T : DaemonService, new()
    {
        Run(new T(), args);
    }

    public static void Run<T>(T service, params string[] args) where T : DaemonService
    {
        var code = new CliParser().Run(args);
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
