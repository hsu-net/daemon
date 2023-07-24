using System.Diagnostics;
using Hsu.Daemon.Windows;
//-:cnd:noEmit
#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
[assembly: SupportedOSPlatform("windows")]
#endif
//+:cnd:noEmit

var source = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule!.FileName)!;
if (!EventLog.SourceExists(source))
{
    EventLog.CreateEventSource(source, "Application");
}

var timer = new Timer(_ =>
{
    WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
},null,TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(5));

DaemonHost
    .Run(x => x
            .OnStart(OnStart)
            .OnStop(OnStop)
        , args);

void OnStart()
{
    timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(5));
    WriteLine("Started");
}

void OnStop()
{
    timer.Dispose();
    WriteLine("Stopped");
}

void WriteLine(string message)
{
    if (Environment.UserInteractive)
    {
        Console.WriteLine(message);
    }

    EventLog.WriteEntry(source!, message, EventLogEntryType.Information);
}