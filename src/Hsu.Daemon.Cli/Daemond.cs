using Hsu.Daemon.Cli;

namespace Hsu.Daemon;

public class Daemond : IDaemond
{
    private readonly string[] _args;
    private readonly Action<ServiceOptions>? _configure;
    private readonly IServiceController _controller;
    private ExitCode _code;

    public ExitCode Code => _code;

    internal Daemond(IServiceController controller, Action<ServiceOptions>? configure, params string[] args)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));
        _configure = configure;
        _args = args;
    }

    public bool Runnable()
    {
        _code = new CliParser(_controller, _configure).Run(_args);
        return _code is ExitCode.Serving or ExitCode.Console;
    }

    public static IDaemondBuilder CreateBuilder(params string[] args)
    {
        return new DaemondBuilder(args);
    }
}