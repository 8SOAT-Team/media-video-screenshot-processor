namespace VideoScreenshot.Application.Driven.MessageBrokers;

public interface IEventBusService
{
    Task Publish<T>(T message, CancellationToken cancellation = default);
}