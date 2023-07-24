using Hsu.Daemon.Hosting;

using Hsu.Daemon.Worker;

if (!Hsu.Daemon.Host.Runnable(args, out var code)) return;

var builder = Host.CreateDefaultBuilder(args);

if (Environment.OSVersion.Platform == PlatformID.Win32NT)
{
    builder = builder.UseWindowsService();
}
else
{
    #if (Net461Chosen)
    throw new NotSupportedException();
    #else
    builder = builder.UseSystemd();
    #endif
}

builder
.ConfigureServices(services =>
{
    services.AddHostedService<Worker>();
});

builder.Build().Run(code);