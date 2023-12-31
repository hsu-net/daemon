﻿using Hsu.Daemon.Cli;
// ReSharper disable ClassNeverInstantiated.Global

namespace Hsu.Daemon;

public record ServiceOptions
{
    public string Bin { get; private set; } = string.Empty;
    public Startup Startup { get; set; } = Startup.Boot;
    public string Name { get; set; } = string.Empty;
    public string Display { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TimeSpan Delay { get; set; } = TimeSpan.FromMinutes(1);

    public ServiceOptions() { }

    public ServiceOptions(string bin, Startup startup, string? name = null, string? description = null, string? display = null, TimeSpan? delay = null)
    {
        if (!File.Exists(bin)) throw new FileNotFoundException("The specified file does not exist", bin);
        if (display.IsNullOrWhiteSpace()) display = null;
        if (description.IsNullOrWhiteSpace()) description = null;

        Bin = bin;
        Name = name ?? Path.GetFileNameWithoutExtension(bin);
        Startup = startup;
        Description = description ?? $"The service for {Name}";
        Display = display ?? Name;
        Delay = delay ?? TimeSpan.FromMinutes(1);
    }
}
