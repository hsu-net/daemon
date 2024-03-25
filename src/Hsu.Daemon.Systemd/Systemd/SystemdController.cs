using Hsu.Daemon.Cli;
using System.Diagnostics;
// ReSharper disable ConvertToUsingDeclaration

namespace Hsu.Daemon.Systemd;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("linux")]
#endif

internal class SystemdController : IServiceController
{
    public bool Install(ServiceOptions options)
    {
        var description = options.Description;
        if (options.Description.IsNullOrWhiteSpace()) description = $"{options.Name} service";

        var host = Process.GetCurrentProcess().MainModule!.FileName;
        var dotnet = host.EndsWith($"{Path.PathSeparator}dotnet", StringComparison.OrdinalIgnoreCase)
            ? host
            : OsHelper.GetDotNet();

        if (dotnet.IsNullOrWhiteSpace()) throw new InvalidOperationException("The `dotnet` path is null");

        using (var writer = new SystemdUnitWriter())
        {
            writer.Name = options.Name;
            writer.Bin = dotnet!;
            writer.Arguments = $"{options.Bin} serving";
            writer.WorkingDirectory = Path.GetDirectoryName(options.Bin)!;
            writer.Description = description;
            if (options.Startup == Startup.Delay)
            {
                writer.Type = "idle";
                if (options.Delay > TimeSpan.Zero)
                {
                    writer.IdleSecond = (int)options.Delay.TotalSeconds;
                }
            }

            writer.Write();
        }

        if (Command($"daemon-reload") != 0) return false;
        if (options.Startup == Startup.Demand) return true;
        return Command($"enable {options.Name}") == 0;
    }

    public bool Uninstall(string name)
    {
        return Command($"disable {name}") == 0;
    }

    public void Start(string name)
    {
        Command($"start {name}");
    }

    public void Stop(string name)
    {
        Command($"stop {name}");
    }

    public void Restart(string name)
    {
        Command($"restart {name}");
    }

    public ServiceStatus Status(string name)
    {
        var ret1 = Command($"is-active {name}", out var output1);

        if (ret1 == 0 && output1.Equals("active", StringComparison.OrdinalIgnoreCase))
        {
            return ServiceStatus.Running;
        }

        if (output1.Equals("activating", StringComparison.OrdinalIgnoreCase)) return ServiceStatus.Starting;
        if (output1.Equals("deactivating", StringComparison.OrdinalIgnoreCase)) return ServiceStatus.Stopping;

        var ret2 = Command($"is-enabled {name}", out var output2);
        if (ret2 == 0 && output2.Trim().Equals("enabled", StringComparison.OrdinalIgnoreCase))
        {
            return ServiceStatus.Stopped;
        }

        return ServiceStatus.Uninstalled;
    }

    private int Command(string args)
    {
        return ProcessHelper.Command("systemctl", args);
    }

    private int Command(string args, out string output)
    {
        return ProcessHelper.Command("systemctl", args, out output);
    }
}
