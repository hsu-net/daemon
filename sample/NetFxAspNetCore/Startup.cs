﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NetFxAspNetCore;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddCheck<RandomHealthCheck>("random", HealthStatus.Unhealthy, new[] { "random","worker" });
        services.AddHostedService<Worker>();
        services.AddRouting();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app)
    {
        app
            .UseEndpointRouting()
            .UseHealthChecks("/healthy");
    }
}