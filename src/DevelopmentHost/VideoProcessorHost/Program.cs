using CommunityToolkit.Aspire.Hosting.Dapr;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.AddProject<Projects.MessagePublisher>("publisher-start-processing");
builder.AddProject<Projects.VideoScreenshot_MessageConsumer>("consumer-processing");

builder.Build().Run();