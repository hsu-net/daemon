using System.Runtime.Versioning;
using System.ServiceProcess;

namespace Hsu.Daemon.Windows;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("windows")]
#endif
public abstract class DaemonService : ServiceBase
{
    public void Start(string[] args) => OnStart(args);
}
