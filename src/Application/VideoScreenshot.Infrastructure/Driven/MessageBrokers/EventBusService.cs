using VideoScreenshot.Application.Configurations;
using VideoScreenshot.Application.Driven.MessageBrokers;
using VideoScreenshot.Infrastructure.Driven.Buckets.Clients;

namespace VideoScreenshot.Infrastructure.Driven.MessageBrokers;

public sealed class EventBusService(IPubSubClientProxy daprClient) : IEventBusService
{
    public Task Publish<T>(T message, CancellationToken cancellation = default) =>
        daprClient.PublishEventAsync(AppConfiguration.PubSubComponent, "video-processed-event", message, cancellation);
}