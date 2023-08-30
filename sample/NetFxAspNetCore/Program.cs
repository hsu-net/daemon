using System;
using Hsu.Daemon.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace NetFxAspNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Hsu.Daemon.Host.Runnable(args, out var code)) return;
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

            var app = builder.Build();
            app.Run(code);
        }
    }
}
