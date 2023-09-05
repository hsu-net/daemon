using Hsu.Daemon;
using Hsu.Daemon.Hosting;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace NetFxAspNetCore
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var daemond = Daemond.CreateBuilder(args).UseWindowsServices().Build();
            if (!daemond.Runnable()) return;
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

            var app = builder.Build();
            app.Run(daemond.Code);
        }
    }
}