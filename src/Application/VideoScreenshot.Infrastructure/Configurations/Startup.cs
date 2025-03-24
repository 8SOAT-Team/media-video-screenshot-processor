using Microsoft.Extensions.DependencyInjection;
using VideoScreenshot.Application.Driven.Buckets;
using VideoScreenshot.Application.Driven.Files;
using VideoScreenshot.Application.Driven.Medias;
using VideoScreenshot.Infrastructure.Driven.Buckets;
using VideoScreenshot.Infrastructure.Driven.Buckets.Clients;
using VideoScreenshot.Infrastructure.Driven.Files;
using VideoScreenshot.Infrastructure.Driven.Medias;

namespace VideoScreenshot.Infrastructure.Configurations;

public static class Startup
{
    public static void AddInfrastructurePorts(this IServiceCollection services)
    {
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IVideoService, VideoService>();
        services.AddSingleton<IBucketReader, BucketReader>();
        services.AddSingleton<IBucketWriter, BucketWriter>();
        
        services.AddInfrastructureClients();
    }

    private static void AddInfrastructureClients(this IServiceCollection services)
    {
        var daprProxy = DaprClientProxyBuilder.ConfigureBuilder(builder =>
            {
                builder.UseHttpEndpoint("http://localhost:3500");
            })
            .Build<BucketClientProxy>();
        
        services.AddSingleton<IBucketClientProxy>(daprProxy);
    }
}