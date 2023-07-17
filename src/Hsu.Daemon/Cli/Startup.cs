namespace Hsu.Daemon.Cli;

[Flags]
public enum Startup
{
    /// <summary>
    /// Start the service when system boot.
    /// </summary>
    Boot,

    /// <summary>
    /// Start the service with delay when system boot.
    /// </summary>
    Delay,

    /// <summary>
    /// Manually start the service.
    /// </summary>
    Demand
}
