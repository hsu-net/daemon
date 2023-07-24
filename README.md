# Hsu.Daemon

[![dev](https://github.com/hsu-net/daemon/actions/workflows/build.yml/badge.svg?branch=dev)](https://github.com/hsu-net/daemon/actions/workflows/build.yml)
[![preview](https://github.com/hsu-net/daemon/actions/workflows/deploy.yml/badge.svg?branch=preview)](https://github.com/hsu-net/daemon/actions/workflows/deploy.yml)
[![main](https://github.com/hsu-net/daemon/actions/workflows/deploy.yml/badge.svg?branch=main)](https://github.com/hsu-net/daemon/actions/workflows/deploy.yml)
[![Nuke Build](https://img.shields.io/badge/nuke-build-yellow.svg)](https://github.com/nuke-build/nuke)
![windows](https://img.shields.io/badge/OS-Windows-blue.svg)
![linux](https://img.shields.io/badge/OS-Linux-blue.svg)


## Package Version

| Name | Source | Stable | Preview |
|---|---|---|---|
| Hsu.Daemon | Nuget | [![NuGet](https://img.shields.io/nuget/v/Hsu.Daemon?style=flat-square)](https://www.nuget.org/packages/Hsu.Daemon) | [![NuGet](https://img.shields.io/nuget/vpre/Hsu.Daemon?style=flat-square)](https://www.nuget.org/packages/Hsu.Daemon) |
| Hsu.Daemon | MyGet | [![MyGet](https://img.shields.io/myget/godsharp/v/Hsu.Daemon?style=flat-square&label=myget)](https://www.myget.org/feed/godsharp/package/nuget/Hsu.Daemon) | [![MyGet](https://img.shields.io/myget/godsharp/vpre/Hsu.Daemon?style=flat-square&label=myget)](https://www.myget.org/feed/godsharp/package/nuget/Hsu.Daemon) |
| Hsu.Daemon.Hosting | Nuget | [![NuGet](https://img.shields.io/nuget/v/Hsu.Daemon.Hosting?style=flat-square)](https://www.nuget.org/packages/Hsu.Daemon.Hosting) | [![NuGet](https://img.shields.io/nuget/vpre/Hsu.Daemon.Hosting?style=flat-square)](https://www.nuget.org/packages/Hsu.Daemon.Hosting) |
| Hsu.Daemon.Hosting | MyGet | [![MyGet](https://img.shields.io/myget/godsharp/v/Hsu.Daemon.Hosting?style=flat-square&label=myget)](https://www.myget.org/feed/godsharp/package/nuget/Hsu.Daemon.Hosting) | [![MyGet](https://img.shields.io/myget/godsharp/vpre/Hsu.Daemon.Hosting?style=flat-square&label=myget)](https://www.myget.org/feed/godsharp/package/nuget/Hsu.Daemon.Hosting) |

## Getting Started

### Self Commands

  ```bash
  # windows
  appName.exe --help
  # Linux
  dotnet appName.dll --help
  ```

### Windows Service(.NET Framework)

  ```ps
  PM> Install-Package Hsu.Daemon
  ```

  ```csharp
  DaemonHost
    .Run(x => x
            .OnStart(OnStart)
            .OnStop(OnStop)
        , args);
  ```
### Worker Service


  ```ps
  PM> Install-Package Hsu.Daemon.Hosting
  ```

  ```csharp
  // 1. To parser arguments 
  if (!Hsu.Daemon.Host.Runnable(args, out var code)) return;
  var builder = Host.CreateDefaultBuilder(args);
  
  // 2. Use middleware
  // builder.UseWindowsService();
  // or
  // builder.UseSystemd();

  // 3. Execute serving or console
  builder.Build().Run(code);
  ```

## License

  MIT