using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;

namespace Hsu.Daemon.Cli;

public partial class CliCommand
{
    private void Serving(InvocationContext ctx) => ctx.ExitCode = (int)ExitCode.Serving;
    private void ConsoleApplication(InvocationContext ctx) => ctx.ExitCode = (int)ExitCode.Console;
}
