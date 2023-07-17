namespace Hsu.Daemon.Cli;

public partial class CliParser
{
    private void Start()
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
                default:
                    return;
            }
            
            var count = 15;
            while (_service.Status(_name) != ServiceStatus.Starting && _service.Status(_name) != ServiceStatus.Running && count > 0)
            {
                count--;
                Thread.Sleep(1000);
            }

            Status();
        }
        catch (Exception ex)
        {
            Exception(ex);
            code = ExitCode.Error;
        }
        finally
        {
            _exitCode = code;
        }
    }

    private void Stop()
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
                    Console.WriteLine($"The service [{_name}] is stopping...");
                    _service.Stop(_name);
                    break;
                case ServiceStatus.Stopping:
                    Console.WriteLine($"The service [{_name}] is stopping");
                    return;
                case ServiceStatus.Stopped:
                    Console.WriteLine($"The service [{_name}] is already stopped");
                    return;
                default:
                    return;
            }

            var count = 15;
            while (_service.Status(_name) != ServiceStatus.Stopped && _service.Status(_name) != ServiceStatus.Stopping && count > 0)
            {
                count--;
                Thread.Sleep(1000);
            }

            Status();
        }
        catch (Exception ex)
        {
            Exception(ex);
            code = ExitCode.Error;
        }
        finally
        {
            _exitCode = code;
        }
    }

    private void Restart()
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
                    break;
                case ServiceStatus.Stopping:
                    Console.WriteLine($"The service [{_name}] is stopping,wait for it to finish");
                    return;
                case ServiceStatus.Stopped:
                    Console.WriteLine($"The service [{_name}] is stopped");
                    return;
                default:
                    return;
            }
            
            Thread.Sleep(3000);
            
            var count = 15;
            while (_service.Status(_name) != ServiceStatus.Starting && _service.Status(_name) != ServiceStatus.Running && count > 0)
            {
                count--;
                Thread.Sleep(1000);
            }
            
            Status();
        }
        catch (Exception ex)
        {
            Exception(ex);
            code = ExitCode.Error;
        }
        finally
        {
            _exitCode = code;
        }
    }
}
