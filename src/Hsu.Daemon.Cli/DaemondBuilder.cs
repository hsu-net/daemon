namespace Hsu.Daemon;

internal class DaemondBuilder : IDaemondBuilder
{
    private readonly string[] _args;
    private Action<ServiceOptions>? _configure;
    private IServiceController? _controller;

    public DaemondBuilder(params string[] args)
    {
        _args = args;
    }

    public IDaemondBuilder Configure(Action<ServiceOptions>? configure)
    {
        if (configure == null) return this;
        if (_configure == null) _configure = configure;
        else _configure += configure;

        return this;
    }

    public IDaemondBuilder UseController(IServiceController controller)
    {
        _controller = controller;
        return this;
    }

    public IDaemond Build()
    {
        if (_controller == null)
            throw new InvalidOperationException("There is no `IServiceController` to use.");
        return new Daemond(_controller, _configure, _args);
    }
}