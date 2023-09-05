using Hsu.Daemon.Systemd;

namespace Hsu.Daemon;

public static class DaemondBuilderExtensions
{
    public static IDaemondBuilder UseSystemd(this IDaemondBuilder builder)
    {
        return builder.UseController(new SystemdController());
    }
}