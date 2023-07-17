using System.Text;
using Hsu.Daemon.Cli;
// ReSharper disable MemberCanBePrivate.Global

namespace Hsu.Daemon.Mac;

internal class LaunchdPropertyListWriter : IDisposable
{
    private readonly string _system;

    public LaunchdPropertyListWriter(string? system = null)
    {
        _system = system ?? "/Library/LaunchDaemons";
    }

    public string Bin { get; set; } = string.Empty;
    public string Arguments { get; set; } = string.Empty;
    public string WorkingDirectory { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ExitTimeOutSecond { get; set; } = 20;
    public Startup Startup { get; set; } = Startup.Boot;
    public int DelaySecond { get; set; } = 60;
    public string Environment { get; set; } = string.Empty;

    public void Write()
    {
        if (Bin.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(Bin));

        if (Name.IsNullOrWhiteSpace())
        {
            Name = Path.GetFileNameWithoutExtension(Bin);
        }

        if (Description.IsNullOrWhiteSpace())
        {
            Description = Name;
        }

        if (WorkingDirectory.IsNullOrWhiteSpace())
        {
            WorkingDirectory = Path.GetDirectoryName(Bin)!;
        }
        
        var xml = $"""
                   <?xml version="1.0" encoding="UTF-8"?>
                   <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
                   <plist version="1.0">
                   <dict>
                       <key>Label</key>
                       <string>{Name}</string>
                       <key>Description</key>
                       <string>{Description}</string>
                       <key>ProgramArguments</key>
                       <array>
                           <string>{Bin}</string>
                           {GetArguments(Arguments)}
                       </array>
                   
                       <key>WorkingDirectory</key>
                       <string>{WorkingDirectory}</string>
                       
                       <key>LaunchOnlyOnce</key>
                       <true/>
                       
                       <key>RunAtLoad</key>
                       {(Startup != Startup.Boot ? "<false/>":"<true/>")}
                       {(Startup== Startup.Delay ? GetDelay(DelaySecond):string.Empty)}
                   
                       {GetEnvironment(Environment)}
                       <key>ExitTimeOut</key>
                       <integer>{ExitTimeOutSecond}</integer>
                       
                       <key>StandardOutPath</key>
                       <string>{Path.Combine(WorkingDirectory,"logs","out.log")}</string>
                       <key>StandardErrorPath</key>
                       <string>{Path.Combine(WorkingDirectory,"logs","error.log")}</string>
                   </dict>
                   </plist>
                   """;
        using var writer = new StreamWriter($"{Path.Combine(_system, $"{Name}.plist")}",false, Encoding.UTF8);
        writer.Write(xml);
        writer.Flush();
    }

    private static string GetDelay(int delay)
    {
        if (delay<1) delay = 20;

        var builder = new StringBuilder();
        builder.AppendLine();
        builder.AppendLine("\t<key>StartInterval</key>");
        builder.AppendLine($"\t<integer>{delay}</integer>");
        // builder.AppendLine("""
        //                    \t<key>StartCalendarInterval</key>
        //                    \t<dict>
        //                    \t    <key>Minute</key>
        //                    \t    <integer>0</integer>
        //                    \t    <key>Hour</key>
        //                    \t    <integer>0</integer>
        //                    \t    <key>Day</key>
        //                    \t    <integer>0</integer>
        //                    \t    <key>Month</key>
        //                    \t    <integer>0</integer>
        //                    \t    <key>Year</key>
        //                    \t    <integer>0</integer>
        //                    \t</dict>
        //                    """
        // );
        builder.AppendLine();
        return builder.ToString();
    }

    private static string GetArguments(string args)
    {
        if (args.IsNullOrWhiteSpace()) args = "serving";

        var builder = new StringBuilder();
        var lines = args.Split(' ');
        foreach(var line in lines)
        {
            if (line.Trim().IsNullOrWhiteSpace()) continue;
            builder.AppendLine($"\t\t<string>{line.Trim()}</string>");
        }

        builder.AppendLine();

        return builder.ToString();
    }

    private static string? GetEnvironment(string environment)
    {
        if (environment.IsNullOrWhiteSpace()) return null;
        var builder = new StringBuilder();
        builder.AppendLine("\t<key>EnvironmentVariables</key>");
        builder.AppendLine("\t<dict>");
        var lines = environment.Split(new[] { System.Environment.NewLine }, StringSplitOptions.None);

        var count = 0;
        foreach(var line in lines)
        {
            var array = line.Split(';');
            foreach(var item in array)
            {
                var var = item.Split('=');
                if (var.Length != 2) continue;
                builder.AppendLine($"\t\t<key>{var[0]}</key>");
                builder.AppendLine($"\t\t<string>{var[1]}</string>");
                count++;
            }
        }

        if (count == 0) return null;

        builder.AppendLine("\t</dict>");
        builder.AppendLine();

        return builder.ToString();
    }

    public void Dispose() { }
}

