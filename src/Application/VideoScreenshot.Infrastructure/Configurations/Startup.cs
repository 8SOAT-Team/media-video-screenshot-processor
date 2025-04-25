using Amazon;
using Amazon.Runtime;
using Amazon.S3;
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
        services.AddSingleton<IBucketClientProxy, BucketClientProxy>();

        services.AddInfrastructureClients(configuration);
    }

    private static void AddInfrastructureClients(this IServiceCollection services, AppConfiguration configuration)
    {
        services.AddSingleton<IAmazonS3>(_ =>
        {
            var config = new AmazonS3Config
            {
                ServiceURL = "http://localhost:4566", // LocalStack
                ForcePathStyle = true,
                RegionEndpoint = RegionEndpoint.USEast1
            };

            var credentials = new BasicAWSCredentials("test", "test");

            return new AmazonS3Client(credentials, config);
        });
    }
}