using System.CommandLine.Invocation;

namespace Hsu.Daemon.Cli;

public partial class CliCommand
{
    private void Start(InvocationContext ctx)
    {
        var code = ExitCode.Success;
        try
        {
            var status = _service.Status(_name);
            switch (status)
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
                    Console.WriteLine($"The service [{_name}] is stopping,wait for it to finish");
                    return;
                case ServiceStatus.Stopped:
                    _service.Start(_name);
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

    private void Stop(InvocationContext ctx)
    {
        var code = ExitCode.Success;
        try
        {
            var status = _service.Status(_name);
            switch (status)
            {
                case ServiceStatus.Uninstalled:
                    Console.WriteLine($"The service [{_name}] is not installed");
                    return;
                case ServiceStatus.Starting:
                case ServiceStatus.Running:
                    _service.Stop(_name);
                    return;
                case ServiceStatus.Stopping:
                    Console.WriteLine($"The service [{_name}] is stopping");
                    return;
                case ServiceStatus.Stopped:
                    Console.WriteLine($"The service [{_name}] is already stopped");
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

    private void Restart(InvocationContext ctx)
    {
        var code = ExitCode.Success;
        try
        {
            var status = _service.Status(_name);
            switch (status)
            {
                case ServiceStatus.Uninstalled:
                    Console.WriteLine($"The service [{_name}] is not installed");
                    return;
                case ServiceStatus.Starting:
                    Console.WriteLine($"The service [{_name}] is starting,wait for it to finish");
                    return;
                case ServiceStatus.Running:
                    _service.Restart(_name);
                    return;
                case ServiceStatus.Stopping:
                    Console.WriteLine($"The service [{_name}] is stopping,wait for it to finish");
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
}
