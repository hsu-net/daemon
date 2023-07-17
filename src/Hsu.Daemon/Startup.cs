using System.Diagnostics;
using System.Reflection;
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace Hsu.Daemon;

public class Startup
{
    private static string _appRoot = string.Empty;
    private Mutex? _mutex;
    private IHost? _host;

    public static void Initialize(string? appRoot)
    {
        _appRoot = Path.GetDirectoryName(appRoot ?? Assembly.GetEntryAssembly()!.Location)!;
        Environment.SetEnvironmentVariable("AppRoot", _appRoot, EnvironmentVariableTarget.Process);

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("serilog.json")
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }

    private void Configure()
    {
        _mutex = new Mutex(true, typeof(Startup).Namespace, out var createdNew);

        if (!createdNew)
        {
            Environment.Exit(1);
            return;
        }

        try
        {
            _host = CreateDefaultBuilder(_appRoot).Build();
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
        }
    }

    private static IHostBuilder CreateDefaultBuilder(string root)
    {
        return Host
            .CreateDefaultBuilder(null)
            .ConfigureHostConfiguration(x => x.SetBasePath(root))
            .ConfigureServices((hostContext, services) =>
            {
                ConfigureServices(services, hostContext.Configuration);
            })
            .UseWindowsService()
            .UseSerilog();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppOptions>(configuration.GetSection("App"));
        services.AddSingleton<IExportService, ExportService>();
        services.AddSingleton<ISyncService, SyncService>();
        services.AddHostedService((sp) => sp.GetRequiredService<ISyncService>());
        ConfigureFreeSql(services, configuration);
    }

    private static void ConfigureFreeSql(IServiceCollection services, IConfiguration configuration)
    {
        var freeSql = new FreeSqlBuilder()
            .UseConnectionString(DataType.MySql, configuration.GetConnectionString("Default"))
            .Build();

        freeSql.Aop.CommandAfter += (s, e) =>
        {
            if (e.Exception != null)
            {
                Log.Logger.Error("Message:{Message}\r\nStackTrace:{StackTrace}", e.Exception.Message, e.Exception.StackTrace);
            }

            Log.Logger.Debug("FreeSql>{Sql}", e.Command.CommandText);
        };

        services.AddSingleton(freeSql);
    }

    public void Run(bool service)
    {
        try
        {
            Configure();
            _host?.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopped.Register(Stop);
            _host?.Run();
            if (service) return;
            _host?.WaitForShutdown();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly!");
        }
    }

    private void Stop()
    {
        try
        {
            _host?.Dispose();
            Log.CloseAndFlush();
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Exit Worker host.");
            throw;
        }
        finally
        {
            Log.Information("Exit Worker host.");
        }

        _mutex?.Dispose();
        Process.GetCurrentProcess().Close();
    }
}