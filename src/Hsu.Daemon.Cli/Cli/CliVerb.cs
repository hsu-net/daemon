using CommandLine;
// ReSharper disable ClassNeverInstantiated.Global

namespace Hsu.Daemon.Cli;

public abstract class CliVerb { }

[Verb("install", false, new[] { "-i", "/i" }, HelpText = "Install this app as a service.")]
public class InstallVerb : CliVerb
{
    [Option('t', "--startup", Required = false, HelpText = "The startup type of service.")]
    public Startup? Startup { get; set; }

    [Option('d', "--description", Required = false, HelpText = "The description of service.")]
    public string Description { get; set; } = string.Empty;

    [Option('s', "--delay", Required = false, HelpText = "The delay seconds of service.")]
    public int? Delay { get; set; }

    [Option('n', "--display", Required = false, HelpText = "The display of the service.")]
    public string Display { get; set; } = string.Empty;
}

[Verb("uninstall", false, new[] { "-u", "/u" }, HelpText = "Uninstall this app service from services.")]
public class UninstallVerb : CliVerb { }

[Verb("status", false, new[] { "-s", "/s" }, HelpText = "Query status of this service.")]
public class StatusVerb : CliVerb { }

[Verb("start", false, new[] { "-t", "/t" }, HelpText = "To start this service.")]
public class StartVerb : CliVerb { }

[Verb("stop", false, new[] { "-p", "/p" }, HelpText = "To stop this service.")]
public class StopVerb : CliVerb { }

[Verb("restart", false, new[] { "-r", "/r" }, HelpText = "To restart this service.")]
public class RestartVerb : CliVerb { }

[Verb("console", false, new[] { "-c", "/c" }, HelpText = "Running this app as console.")]
public class ConsoleVerb : CliVerb { }

[Verb("serving", false, new[] { "-v", "/v" }, HelpText = "Running this app as service.")]
public class ServingVerb : CliVerb { }