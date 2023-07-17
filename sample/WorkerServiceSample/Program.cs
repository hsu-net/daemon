using Hsu.Daemon.Hosting;

using WorkerServiceSample;

if (!Hsu.Daemon.Host.Runnable(args, out var code)) return;

var builder = Host.CreateDefaultBuilder(args);

if (Environment.OSVersion.Platform == PlatformID.Win32NT)
{
    builder = builder.UseWindowsService();
}
else
{
#if NET5_0_OR_GREATER || NETCOREAPP2_1_OR_GREATER
    builder = builder.UseSystemd();
#endif
}

builder
.ConfigureServices(services =>
{
    services.AddHostedService<Worker>();
});

IHost host = builder.Build();
host.Run(code);