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

public sealed class LambdaService(Action onStart, Action onStop, Action? onDispose = null) : DaemonService
{
    protected override void OnStart(string[] args)
    {
        onStart.Invoke();
        base.OnStart(args);
    }

    protected override void OnStop()
    {
        onStop.Invoke();
        base.OnStop();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        onDispose?.Invoke();
    }
}
