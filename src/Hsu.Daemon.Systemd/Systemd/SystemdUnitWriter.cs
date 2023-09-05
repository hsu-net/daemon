using System.Text;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Hsu.Daemon.Systemd;

internal sealed record SystemdUnitWriter : IDisposable
{
    private readonly string _system;

    public SystemdUnitWriter(string? system = null)
    {
        _system = system ?? "/etc/systemd/system";
    }

    public string Bin { get; set; } = string.Empty;
    public string Arguments { get; set; } = string.Empty;
    public string WorkingDirectory { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TimeoutStartSecond { get; set; } = 0;

    /// <summary>
    /// no | always | on-failure | on-abnormal | on-abort | on-success
    /// </summary>
    public string Restart { get; set; } = "on-failure";
    
    /// <summary>
    /// simple | notify | idle  | forking | oneshot | dbus
    /// </summary>
    public string Type { get; set; } = "notify";
    
    public int IdleSecond { get; set; } = 60;
    public int RestartSecond { get; set; } = 5;
    public string Dependencies { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string EnvironmentFile { get; set; } = string.Empty;

    public void Write()
    {
        if (Bin.IsNullOrWhiteSpace()) throw new InvalidDataException("The properity `Bin` is null");

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

        using var writer = new StreamWriter($"{Path.Combine(_system, $"{Name}.service")}",false, Encoding.UTF8);

        writer.WriteLine("[Unit]");
        writer.WriteLine($"Type={Type}");
        writer.WriteLine($"Description={Description}");
        if (!Dependencies.IsNullOrWhiteSpace())
        {
            writer.WriteLine($"After={Dependencies}");
        }

        writer.WriteLine();
        writer.WriteLine("[Service]");
        writer.WriteLine($"WorkingDirectory={WorkingDirectory}");
        writer.WriteLine($"ExecStart={Bin} {Arguments}");
        writer.WriteLine($"TimeoutStartSec={TimeoutStartSecond}");
        writer.WriteLine($"Restart={Restart}");
        writer.WriteLine($"RestartSec={RestartSecond}");
        if (Type.Equals("idle", StringComparison.OrdinalIgnoreCase) && IdleSecond > 0)
        {
            writer.WriteLine($"IdleSec={IdleSecond}");
        }

        if (!Environment.IsNullOrWhiteSpace())
        {
            writer.WriteLine($"Environment={Environment}");
        }

        if (!EnvironmentFile.IsNullOrWhiteSpace())
        {
            writer.WriteLine($"EnvironmentFile={EnvironmentFile}");
        }

        writer.WriteLine();
        writer.WriteLine("[Install]");
        writer.WriteLine("WantedBy=multi-user.target");
        writer.Flush();
    }
    
    public void Dispose() { }
}
