using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using VideoScreenshot.Application.Configurations;
using VideoScreenshot.Application.Driven.Buckets;
using VideoScreenshot.Application.Driven.Files;
using VideoScreenshot.Application.Driven.Medias;
using VideoScreenshot.Application.Driven.MessageBrokers;
using VideoScreenshot.Infrastructure.Driven.Buckets;
using VideoScreenshot.Infrastructure.Driven.Buckets.Clients;
using VideoScreenshot.Infrastructure.Driven.Files;
using VideoScreenshot.Infrastructure.Driven.Medias;
using VideoScreenshot.Infrastructure.Driven.MessageBrokers;

namespace VideoScreenshot.Infrastructure.Configurations;

public static class Startup
{
    public static void AddInfrastructurePorts(this IServiceCollection services, AppConfiguration configuration)
    {
        services.AddSingleton<IVideoService, VideoService>();
        services.AddSingleton<IBucketReader, BucketReader>();
        services.AddSingleton<IBucketWriter, BucketWriter>();
        services.AddSingleton<IPackFileService, PackFileService>();
        services.AddSingleton<IEventBusService, EventBusService>();

        services.AddInfrastructureClients(configuration);
    }

    private static void AddInfrastructureClients(this IServiceCollection services, AppConfiguration configuration)
    {
        var daprBucketProxy = DaprClientProxyBuilder.ConfigureBuilder(builder =>
            {
                builder.UseHttpEndpoint(configuration.DaprHttpUrl);
                builder.UseGrpcEndpoint(configuration.DaprGrpcUrl);
            })
            .Build<BucketClientProxy>();

        services.AddSingleton<IBucketClientProxy>(daprBucketProxy);

        var daprPubSubClientProxy = DaprClientProxyBuilder.ConfigureBuilder(builder =>
            {
                builder.UseHttpEndpoint(configuration.DaprHttpUrl);
                builder.UseGrpcEndpoint(configuration.DaprGrpcUrl);
            })
            .Build<PubSubClientProxy>();

        services.AddSingleton<IPubSubClientProxy>(daprPubSubClientProxy);
    }
}