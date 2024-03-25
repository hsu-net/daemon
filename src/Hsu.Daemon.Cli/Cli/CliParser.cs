using CommandLine;
using CommandLine.Text;
using System.Diagnostics;

namespace Hsu.Daemon.Cli;

public partial class CliParser
{
    private static readonly Type[] Verbs = [typeof(InstallVerb),typeof(UninstallVerb),typeof(StatusVerb),typeof(StartVerb),typeof(StopVerb),typeof(RestartVerb),typeof(ConsoleVerb),typeof(ServingVerb)];
    
    private readonly string _bin;
    private readonly string _name;
    private readonly string? _description;
    private readonly string? _display;
    private readonly int? _delay;
    private readonly Startup _startup;
    private readonly IServiceController _service;
    private readonly Type[] _types;
    private readonly Parser _parser;
    
    private ExitCode _exitCode;
    
    public CliParser(IServiceController serviceController, Action<ServiceOptions>? configure = null)
    {
        if (OsHelper.IsWindows())
        {
            var host = Process.GetCurrentProcess().MainModule!.FileName;
            _bin = host.EndsWith("dotnet.exe") ? Environment.GetCommandLineArgs()[0] : host;
        }
        else
        {
            _bin = Environment.GetCommandLineArgs()[0];
        }

        var options = new ServiceOptions();
        configure?.Invoke(options);

        _name = !options.Name.IsNullOrWhiteSpace()
            ? options.Name
            : Path.GetFileNameWithoutExtension(_bin);

        if (!options.Description.IsNullOrWhiteSpace())
        {
            _description = options.Description;
        }

        if (!options.Display.IsNullOrWhiteSpace())
        {
            _display = options.Display;
        }

        if (options.Delay > TimeSpan.Zero)
        {
            _delay = (int)options.Delay.TotalSeconds;
        }

        _startup = configure == null ? Startup.Boot : options.Startup;

        _service = serviceController;

        _types = Verbs;

        _parser = new Parser(x =>
        {
            x.EnableDashDash = true;
            x.AutoHelp = true;
            x.AutoVersion = true;
            x.HelpWriter = null;
        });
    }

    private Task RunAsync(CliVerb verb)
    {
        switch (verb)
        {
            case ServingVerb:
            {
                Serving();
                break;
            }
            case ConsoleVerb:
            {
                ConsoleApplication();
                break;
            }
            case InstallVerb vb:
            {
                Install(new ServiceOptions(_bin
                    , vb.Startup ?? _startup
                    , _name
                    , vb.Description.IsNullOrWhiteSpace() ? _description : vb.Description
                    , vb.Display.IsNullOrWhiteSpace() ? _display : vb.Display
                    , TimeSpan.FromSeconds(vb.Delay ?? _delay ?? -1)
                ));
                break;
            }
            case UninstallVerb:
            {
                Uninstall();
                break;
            }
            case StatusVerb:
            {
                Status();
                break;
            }
            case StartVerb:
            {
                Start();
                break;
            }
            case StopVerb:
            {
                Stop();
                break;
            }
            case RestartVerb:
            {
                Restart();
                break;
            }
        }

        Console.Out.Flush();
        #if NET45
        return Task.FromResult(true);
        #else
        return Task.CompletedTask;
        #endif
    }

    private void Run(CliVerb verb) => RunAsync(verb).ConfigureAwait(false).GetAwaiter().GetResult();

    private void Exception(Exception ex)
    {
        Console.Error.WriteLine($"Message : {ex.Message}");
        Console.Error.WriteLine($"StackTrace : {ex.StackTrace}");
    }

    private static void DisplayHelp(ParserResult<object> result, IEnumerable<Error> errs)
    {
        string helpText;
        if (errs.IsVersion())
        {
            helpText = HelpText.AutoBuild(result);
        }
        else
        {
            helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = false;
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e, true);
        }

        Console.WriteLine(helpText);
    }

    public ExitCode Run(string[] args)
    {
        var pr = _parser.ParseArguments(args.Length == 0 ? new[] { "--help" } : args, _types);
        pr.WithParsed<CliVerb>(Run);
        pr.WithNotParsed(e => DisplayHelp(pr, e));
        return _exitCode;
    }

    public async Task<ExitCode> RunAsync(string[] args)
    {
        var pr = _parser.ParseArguments(args.Length == 0 ? new[] { "--help" } : args, _types);
        pr.WithNotParsed(e => DisplayHelp(pr, e));
        await pr.WithParsedAsync<CliVerb>(RunAsync);
        return _exitCode;
    }
}
