namespace VideoScreenshot.Infrastructure.Driven.Buckets.Clients;

public interface IBucketClientProxy : IDaprClientProxy;

public interface IPubSubClientProxy : IDaprClientProxy
{
    Task PublishEventAsync<TData>(
        string pubsubName,
        string topicName,
        TData data,
        CancellationToken cancellationToken = default);
}