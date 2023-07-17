namespace Hsu.Daemon.Cli;

public partial class CliParser
{
    private void Install(ServiceOptions options)
    {
        var code = ExitCode.Success;
        try
        {
            var status = _service.Status(_name);
            if (status == ServiceStatus.Uninstalled)
            {
                if (_service.Install(options))
                {
                    Console.WriteLine($"The service [{_name}] install  successful");
                }
                else
                {
                    Console.Error.WriteLine($"The service [{_name}] install failed");
                }
            }
            else
            {
                Console.WriteLine($"The service [{_name}] already installed");
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

    private void Uninstall()
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
                    break;
            }

            if (_service.Uninstall(_name))
            {
                Console.WriteLine($"The service [{_name}] uninstall  successful");
            }
            else
            {
                Console.Error.WriteLine($"The service [{_name}] uninstall failed");
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
