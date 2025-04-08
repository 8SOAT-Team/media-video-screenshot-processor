using CommunityToolkit.Aspire.Hosting.Dapr;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

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
        DaprGrpcPort = int.Parse(builder.Configuration["AppConfiguration:DaprGrpcPort"] ?? "3001"),
        DaprHttpPort = int.Parse(builder.Configuration["AppConfiguration:DaprHttpPort"] ?? "3000"),
        AppMaxConcurrency = 1,
    });

// var envs = builder.Configuration.GetSection("AppConfiguration").GetChildren()
//     .Select(x => new KeyValuePair<string, string>(x.Key, x.Value!))
//     .ToDictionary(x => x.Key, x => x.Value);
//
// var consumer = builder.AddContainer("consumer-processing", "videoscreenshot.messageconsumer:latest")
//     .WithEndpoint(port: 5148, targetPort: 5148, scheme: "http");
//
// foreach (var env in envs)
// {
//     consumer.WithEnvironment($"AppConfiguration__{env.Key}", env.Value);
// }
//
// consumer.WithDaprSidecar(new DaprSidecarOptions
// {
//     AppId = "processing-video-consumer",
//     AppPort = 5148,
//     DaprGrpcPort = 3000,
//     DaprHttpPort = 3001,
//     AppMaxConcurrency = 1,
// });


// builder.AddContainer("localstack", "localstack/localstack:latest")
//     .WithEndpoint(port: 4566, targetPort: 4566, scheme: "http")
//     .WithEnvironment("DOCKER_HOST", "unix:///var/run/docker.sock")
//     .WithEnvironment("LOCALSTACK_API_KEY", "test")
//     .WithEnvironment("LOCALSTACK_HOSTNAME", "localstack")
//     .WithEnvironment("AWS_REGION", "us-east-1")
//     .WithEnvironment("DEBUG", "${DEBUG:-1}")
//     .WithEnvironment("LOCALSTACK_UI", "1")
//     .WithEnvironment("SERVICES", "s3,sqs,sns,sts")
//     .WithVolume("./localstack_data:/var/lib/localstack/data")
//     .WithVolume("/var/run/docker.sock:/var/run/docker.sock");

builder.Build().Run();