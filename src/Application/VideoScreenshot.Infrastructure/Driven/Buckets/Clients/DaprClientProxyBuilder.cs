using Dapr.Client;

namespace VideoScreenshot.Infrastructure.Driven.Buckets.Clients;

public class DaprClientProxyBuilder(Action<DaprClientBuilder> builder)
{
    private Action<DaprClientBuilder> _builder = builder;

    public TClientProxy Build<TClientProxy>() where TClientProxy : DaprClientProxy
    {
        var client = new DaprClientBuilder();
        _builder(client);

        return (TClientProxy)Activator.CreateInstance(typeof(TClientProxy), client.Build())!;
    }

    public static DaprClientProxyBuilder ConfigureBuilder(Action<DaprClientBuilder> builder)
    {
        return new DaprClientProxyBuilder(builder);
    }
}