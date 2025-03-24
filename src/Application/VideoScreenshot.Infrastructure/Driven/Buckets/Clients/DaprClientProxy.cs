using Dapr.Client;

namespace VideoScreenshot.Infrastructure.Driven.Buckets.Clients;

public abstract class DaprClientProxy(DaprClient client) : IDaprClientProxy
{
   public Task<BindingResponse> InvokeBindingAsync(BindingRequest request,
        CancellationToken cancellationToken = default)
    {
        return client.InvokeBindingAsync(request, cancellationToken);
    }
}