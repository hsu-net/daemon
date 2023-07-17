using CommandLine;
using CommandLine.Text;

using Hsu.Daemon.Linux;
using Hsu.Daemon.Windows;

using System.Diagnostics;
using System.Reflection;

namespace Hsu.Daemon.Cli;

public partial class CliParser
{
    private readonly string _bin;
    private readonly string _name;
    private readonly IServiceController _service;
    private readonly Type[] _types;
    private readonly Parser _parser;

    private ExitCode _exitCode;

    public CliParser()
    {
        _bin = OsHelper.IsWindows() ? Process.GetCurrentProcess().MainModule!.FileName! : Environment.GetCommandLineArgs()[0];
        _name = Path.GetFileNameWithoutExtension(_bin);

        _service = OsHelper.IsWindows()
                ? new ServiceControlController()
                : new SystemdController();
        _types = LoadVerbs();

        _parser = new Parser(x =>
        {
            x.EnableDashDash = true;
            x.AutoHelp = true;
            x.AutoVersion = true;
            x.HelpWriter = null;
        });
    }

    private static Type[] LoadVerbs()
    {
        return typeof(CliParser).Assembly.GetTypes()
            .Where(t => typeof(CliVerb).IsAssignableFrom(t) && t.GetCustomAttribute<VerbAttribute>() != null).ToArray();
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
                    Install(new ServiceOptions(_bin, vb.Startup, _name, vb.Description, vb.Display, TimeSpan.FromSeconds(vb.Delay)));
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