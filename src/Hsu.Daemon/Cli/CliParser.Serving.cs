namespace Hsu.Daemon.Cli;

public partial class CliParser
{
    private void Serving() => _exitCode = ExitCode.Serving;
    private void ConsoleApplication() => _exitCode = ExitCode.Console;
    
    private void Status()
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
            _exitCode = code;
        }
    }
}
