using CommunityToolkit.Aspire.Hosting.Dapr;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MessagePublisher>("publisher-start-processing")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "publisher-api"
    });

builder.AddProject<Projects.VideoScreenshot_MessageConsumer>("consumer-processing")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "processing-video-consumer",
        AppPort = 5148,
        DaprGrpcPort = 3000,
        DaprHttpPort = 3001,
        AppMaxConcurrency = 1,
    });

builder.Build().Run();