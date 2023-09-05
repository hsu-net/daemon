using Hsu.Daemon.Cli;

namespace Hsu.Daemon;

public interface IDaemond
{
    ExitCode Code { get; }

    bool Runnable();
}