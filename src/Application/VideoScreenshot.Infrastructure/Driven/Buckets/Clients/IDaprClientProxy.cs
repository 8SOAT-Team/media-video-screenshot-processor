using Dapr.Client;

namespace VideoScreenshot.Infrastructure.Driven.Buckets.Clients;

public interface IDaprClientProxy
{
    Task<BindingResponse> InvokeBindingAsync(BindingRequest request,
        CancellationToken cancellationToken = default);
}