using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Hsu.Daemon.Linux;
using Hsu.Daemon.Windows;
// ReSharper disable UnusedMember.Global

namespace Hsu.Daemon.Cli;

public partial class CliCommand
{
    private readonly IServiceController _service;
    private readonly RootCommand _command;
    private readonly string _name;

    public CliCommand()
    {
        var bin = Process.GetCurrentProcess().MainModule!.FileName!;
        _name = Path.GetFileNameWithoutExtension(bin);
        _service =
            #if NET5_0_OR_GREATER
            OperatingSystem.IsWindows()
            #else
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            #endif
                ? new WinScController()
                : new SystemdController();

        var root = new RootCommand("The app for System.CommandLine");

        var install = new Command("install", "Install this app as a service.");
        install.AddAlias("i");

        var startup = new Option<Startup>(new[] { "--startup", "-t" }, "The startup type of the service.");
        var description = new Option<string>(new[] { "--description", "-d" }, "The description of the service.");
        var delay = new Option<int>(new[] { "--delay", "-s" }, "The delay seconds of the service.");
        var display = new Option<string>(new[] { "--display", "-n" }, "The display of the service.");
        install.AddOption(startup);
        install.AddOption(description);
        install.AddOption(delay);

        #if NET5_0_OR_GREATER
        if (OperatingSystem.IsWindows())
        #else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        #endif
        {
            install.AddOption(display);
        }

        install.SetHandler((ctx) => Install(ctx, new ServiceOptionsBinder(bin, _name, startup, description, delay, display)));
        root.AddCommand(install);

        var uninstall = new Command("uninstall", "Uninstall this app service from services.");
        uninstall.AddAlias("u");
        uninstall.SetHandler(Uninstall);
        root.AddCommand(uninstall);

        var start = new Command("start", "To start this app service.");
        start.AddAlias("a");
        uninstall.SetHandler(Start);
        root.AddCommand(start);

        var stop = new Command("stop", "To stop this app service.");
        stop.AddAlias("p");
        uninstall.SetHandler(Stop);
        root.AddCommand(stop);

        var restart = new Command("restart", "To restart this app service.");
        restart.AddAlias("r");
        uninstall.SetHandler(Restart);
        root.AddCommand(restart);

        var status = new Command("status", "To query status of this app service.");
        status.AddAlias("s");
        uninstall.SetHandler(Status);
        root.AddCommand(status);

        var srv = new Command("serving", "Run this app as service.");
        srv.AddAlias("v");
        srv.SetHandler(Serving);
        root.AddCommand(srv);

        var console = new Command("console", "Run this app as console.");
        console.AddAlias("c");
        console.SetHandler(ConsoleApplication);
        root.AddCommand(console);

        root.SetHandler(Default);

        _command = root;
    }

    private void Default(InvocationContext ctx)
    {
        ctx.ExitCode = (int)ExitCode.Default;
    }

    private void Exception(Exception ex)
    {
        Console.Error.WriteLine($"Message : {ex.Message}");
        Console.Error.WriteLine($"StackTrace : {ex.StackTrace}");
    }

    private void Status(InvocationContext ctx)
    {
        var code = ExitCode.Success;
        try
        {
            switch (_service.Status(_name))
            {
                case ServiceStatus.Uninstalled:
                    Console.WriteLine($"The service [{_name}] is not installed");
                    return;
                case ServiceStatus.Starting:
                    Console.WriteLine($"The service [{_name}] is starting");
                    return;
                case ServiceStatus.Running:
                    Console.WriteLine($"The service [{_name}] is running");
                    return;
                case ServiceStatus.Stopping:
                    Console.WriteLine($"The service [{_name}] is stopping");
                    return;
                case ServiceStatus.Stopped:
                    Console.WriteLine($"The service [{_name}] is stopped");
                    break;
            }
        }
        catch (Exception ex)
        {
            Exception(ex);
            code = ExitCode.Error;
        }
        finally
        {
            ctx.ExitCode = (int)code;
        }
    }

    public int Run(string[] args) => _command.Invoke(args.Length == 0 ? new[] { "--help" } : args);

    public Task<int> RunAsync(string[] args) => _command.InvokeAsync(args.Length == 0 ? new[] { "--help" } : args);
}
