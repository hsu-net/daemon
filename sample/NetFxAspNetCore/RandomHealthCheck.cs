using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NetFxAspNetCore;

public class RandomHealthCheck:IHealthCheck
{
    private readonly Random _random = new Random((int)DateTime.Now.Ticks);
    
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        var random = _random.Next(0, 100);
        if (random % 2 == 0)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy());
        }
        
        if (random % 5 == 0)
        {
            return Task.FromResult(HealthCheckResult.Degraded());
        }

        return Task.FromResult(HealthCheckResult.Healthy());
    }
}
