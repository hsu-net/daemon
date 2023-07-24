#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
#endif
using System.Diagnostics;
using Hsu.Daemon.Cli;

// ReSharper disable UnusedMember.Local

namespace Hsu.Daemon.Windows;

/// <summary>
/// Service Control Command implementation
/// </summary>
#if NET5_0_OR_GREATER
[SupportedOSPlatform("windows")]
#endif
public sealed class ServiceControlController : IServiceController
{
    private static readonly string Bin = Path.Combine(Environment.SystemDirectory, "sc.exe");

    public bool Install(ServiceOptions options)
    {
        var start = options.Startup switch
        {
            Startup.Boot => "auto",
            Startup.Demand => "demand",
            _ => "delayed-auto"
        };

        var host = Process.GetCurrentProcess().MainModule!.FileName!;
        var bin = host.EndsWith("dotnet.exe", StringComparison.OrdinalIgnoreCase) ? $"{host} {options.Bin}" : options.Bin;

        var installCommand = $"create {options.Name} binPath=\"{bin} serving\" DisplayName=\"{options.Display}\" start={start}";
        if (Command(installCommand) != 0) return false;

        return Command($"description {options.Name} \"{options.Description}\"") == 0;
    }

    public bool Uninstall(string name)
    {
        return Command($"delete {name}") == 0;
    }

    public void Start(string name)
    {
        Command($"start {name}");
        
        // using var serviceController = new Controller(name);
        // serviceController.Start();
        // serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
    }

    public void Stop(string name)
    {
        Command($"stop {name}");
        
        // using var serviceController = new Controller(name);
        // if (serviceController.Status == ServiceControllerStatus.Stopped) return;
        //
        // serviceController.Stop();
        // serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
    }

    public void Restart(string name)
    {
        Command($"restart {name}");
        
        // using var serviceController = new Controller(name);
        // serviceController.Stop();
        // serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
        // serviceController.Start();
        // serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
    }

    public ServiceStatus Status(string name)
    {
        var ret = Command($"query {name}", out var output);
        
        if(ret==1060) return ServiceStatus.Uninstalled;
        if (!output.Contains(name)) return ServiceStatus.Failed;

        if (output.Contains("START_PENDING")) return ServiceStatus.Starting;
        if (output.Contains("RUNNING")) return ServiceStatus.Running;
        if (output.Contains("STOP_PENDING")) return ServiceStatus.Stopping;
        if (output.Contains("STOPPED")) return ServiceStatus.Stopped;
        
        return ServiceStatus.Failed;
        
        // using var serviceController = new Controller(name);
        // try
        // {
        //     return serviceController.Status switch
        //     {
        //         ServiceControllerStatus.Running => ServiceStatus.Running,
        //         ServiceControllerStatus.Stopped => ServiceStatus.Stopped,
        //         ServiceControllerStatus.StartPending => ServiceStatus.Starting,
        //         ServiceControllerStatus.StopPending => ServiceStatus.Stopping,
        //         _ => ServiceStatus.Failed
        //     };
        // }
        // catch (Exception)
        // {
        //     return ServiceStatus.Uninstalled;
        // }
    }

    private int Command(string args)
    {
        return ProcessHelper.Command(Bin, args);
    }

    private int Command(string args, out string output)
    {
        return ProcessHelper.Command(Bin, args, out output);
    }
}
