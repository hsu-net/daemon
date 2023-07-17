﻿using Hsu.Daemon.Cli;

namespace Hsu.Daemon;

public static class Host
{
    public static bool Runnable(string[] args, out ExitCode code)
    {
        code = new CliParser().Run(args);
        return code is ExitCode.Serving or ExitCode.Console;
    }
}