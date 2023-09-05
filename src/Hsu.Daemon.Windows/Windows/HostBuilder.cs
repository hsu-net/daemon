// ReSharper disable MemberCanBePrivate.Global

namespace Hsu.Daemon.Windows;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("windows")]
#endif

public sealed class HostBuilder
{
    private Action? _start;
    private Action? _stop;
    private Action? _dispose;

    public HostBuilder OnStart(Action configure)
    {
        _start = configure;
        return this;
    }

    public HostBuilder OnStop(Action configure)
    {
        _stop = configure;
        return this;
    }

    public HostBuilder OnDispose(Action configure)
    {
        _dispose = configure;
        return this;
    }

    public DaemonService Build()
    {
        if (_start == null) throw new InvalidOperationException("The start action is required");
        if (_stop == null) throw new InvalidOperationException("The stop action is required");
        return new LambdaService(_start, _stop, _dispose);
    }
}

public sealed class LambdaService : DaemonService
{
    private readonly Action _onStart;
    private readonly Action _onStop;
    private readonly Action? _onDispose;

    public LambdaService(Action onStart, Action onStop, Action? onDispose = null)
    {
        _onStart = onStart;
        _onStop = onStop;
        _onDispose = onDispose;
    }

    protected override void OnStart(string[] args)
    {
        _onStart.Invoke();
        base.OnStart(args);
    }

    protected override void OnStop()
    {
        _onStop.Invoke();
        base.OnStop();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _onDispose?.Invoke();
    }
}