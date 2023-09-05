namespace Hsu.Daemon;

public interface IServiceController
{
    bool Install(ServiceOptions options);

    bool Uninstall(string name);

    void Start(string name);

    void Stop(string name);

    void Restart(string name);

    ServiceStatus Status(string name);
}