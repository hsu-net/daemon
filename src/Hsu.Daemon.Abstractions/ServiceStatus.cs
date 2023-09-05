namespace Hsu.Daemon;

public enum ServiceStatus
{
    /// <summary>
    /// The service is not installed
    /// </summary>
    Uninstalled,

    /// <summary>
    /// The service is starting
    /// </summary>
    Starting,

    /// <summary>
    /// The service is running
    /// </summary>
    Running,

    /// <summary>
    /// The service is stopping
    /// </summary>
    Stopping,

    /// <summary>
    /// The service is stopped
    /// </summary>
    Stopped,

    /// <summary>
    /// The service is failed to get status
    /// </summary>
    Failed
}
