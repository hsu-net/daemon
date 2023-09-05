namespace Hsu.Daemon;

public interface IDaemondBuilder
{
    IDaemondBuilder Configure(Action<ServiceOptions> configure);

    IDaemondBuilder UseController(IServiceController controller);

    IDaemond Build();
}