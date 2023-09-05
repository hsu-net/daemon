using Hsu.Daemon;
using Hsu.Daemon.Hosting;

using WorkerServiceSample;

var daemond = Daemond.CreateBuilder(args).UseWindowsServices().Build();
if (!daemond.Runnable()) return;

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

builder.Build().Run(daemond.Code);