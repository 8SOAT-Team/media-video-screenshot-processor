using Dapr.Client;
using VideoScreenshot.Infrastructure.Driven.Buckets.Clients;

namespace VideoScreenshot.Infrastructure.Driven.Buckets;

public sealed class BucketClientProxy(DaprClient client) : DaprClientProxy(client), IBucketClientProxy;
public sealed class PubSubClientProxy(DaprClient client) : DaprClientProxy(client), IPubSubClientProxy
{
    private readonly DaprClient _client = client;

    public Task PublishEventAsync<TData>(string pubsubName, string topicName, TData data,
        CancellationToken cancellationToken = default)
    {
        return _client.PublishEventAsync(pubsubName, topicName, data, cancellationToken);
    }
}