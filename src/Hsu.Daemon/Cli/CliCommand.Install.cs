using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.Runtime.InteropServices;

namespace Hsu.Daemon.Cli;

public partial class CliCommand
{
    private class ServiceOptionsBinder
    {
        private readonly string _bin;
        private readonly string _name;
        private readonly Option<Startup> _startup;
        private readonly Option<string> _description;
        private readonly Option<int> _delay;
        private readonly Option<string> _display;

        public ServiceOptionsBinder(string bin, string name, Option<Startup> startup, Option<string> description, Option<int> delay, Option<string> display)
        {
            _bin = bin;
            _name = name;
            _startup = startup;
            _description = description;
            _delay = delay;
            _display = display;
        }

        public ServiceOptions GetOptions(BindingContext ctx)
        {
            #if NET5_0_OR_GREATER
            if (OperatingSystem.IsWindows())
            #else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            #endif
            {
                return new ServiceOptions(_bin
                    , ctx.ParseResult.GetValueForOption(_startup)
                    , _name
                    , ctx.ParseResult.GetValueForOption(_description)
                    , ctx.ParseResult.GetValueForOption(_display)
                    , TimeSpan.FromSeconds(ctx.ParseResult.GetValueForOption(_delay))
                );
            }

            return new ServiceOptions(_bin
                , ctx.ParseResult.GetValueForOption(_startup)
                , _name
                , ctx.ParseResult.GetValueForOption(_description)
                , null
                , TimeSpan.FromSeconds(ctx.ParseResult.GetValueForOption(_delay))
            );
        }
    }

    private void Install(InvocationContext ctx, ServiceOptionsBinder binder)
    {
        var options = binder.GetOptions(ctx.BindingContext);
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
            ctx.ExitCode = (int)code;
        }
    }

    private void Uninstall(InvocationContext ctx)
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
            ctx.ExitCode = (int)code;
        }
    }
}
