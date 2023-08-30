using System.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace NetFxAspNetCore;

public class Worker : BackgroundService
{
    private readonly string _source;
    private readonly TimeSpan _span;
    private long _counter;

    public Worker()
    {
        _source = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule!.FileName)!;
        _span = TimeSpan.FromSeconds(15);
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        if (!EventLog.SourceExists(_source))
        {
            EventLog.CreateEventSource(_source, "Application");
        }

        WriteLine("Start ...");
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        WriteLine("Stop ...");
        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        WriteLine("Execute ...");

        while (!stoppingToken.IsCancellationRequested)
        {
            Interlocked.Increment(ref _counter);
            WriteLine($"Execute Counter : {_counter}");
            await Task.Delay(_span, stoppingToken);
        }
    }

    void WriteLine(string message)
    {
        if (Environment.UserInteractive)
        {
            Console.WriteLine(message);
        }

        EventLog.WriteEntry(_source!, message, EventLogEntryType.Information);
    }
}
