#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
#endif

using Hsu.Daemon.Cli;

namespace Hsu.Daemon.Windows;
#if NET5_0_OR_GREATER
[SupportedOSPlatform("windows")]
#endif

internal sealed class DaemonServiceBuilder : IDaemonServiceBuilder
{
    internal ExitCode Code;

    internal DaemonService Service;

    internal DaemonServiceBuilder(ExitCode code, DaemonService service)
    {
        Code = code;
        Service = service;
    }
}