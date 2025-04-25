using VideoScreenshot.Domain.Results;

namespace VideoScreenshot.Application.Driven.MessageBrokers;

public interface IEventBusService
{
    Task<OperationResult> Publish<T>(T message, CancellationToken cancellation = default);
}