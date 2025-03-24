using Dapr.Client;
using VideoScreenshot.Infrastructure.Driven.Buckets.Clients;

namespace VideoScreenshot.Infrastructure.Driven.Buckets;

public sealed class BucketClientProxy(DaprClient client) : DaprClientProxy(client), IBucketClientProxy;