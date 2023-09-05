using Hsu.Daemon.Windows;

namespace Hsu.Daemon;

public static class DaemondBuilderExtensions
{
    public static IDaemondBuilder UseWindowsServices(this IDaemondBuilder builder)
    {
        return builder.UseController(new ServiceControlController());
    }
}