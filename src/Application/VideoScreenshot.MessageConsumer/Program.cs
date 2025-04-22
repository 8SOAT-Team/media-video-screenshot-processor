using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VideoScreenshot.Application.Configurations;
using VideoScreenshot.Application.Drivers;
using VideoScreenshot.Application.Requests;
using VideoScreenshot.Infrastructure.Configurations;
using VideoScreenshot.MessageConsumer;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var environment = builder.Environment.EnvironmentName;
Console.WriteLine($"[Startup] ASPNETCORE_ENVIRONMENT: {environment}");

var appConfiguration = builder.Configuration.GetSection(AppConfiguration.ConfigurationSectionName)
    .Get<AppConfiguration>();

builder.Services.AddSingleton(appConfiguration!);

builder.Services
    .AddSingleton<IValidateOptions<AppConfiguration>, ValidateSettingsOptions>();

builder.Services.AddApplicationPorts(appConfiguration!.FfmpegPath);
builder.Services.AddInfrastructurePorts(appConfiguration);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseCloudEvents();

app.MapSubscribeHandler();

app.MapPost("/video-start-processing",
    [Topic(AppConfiguration.PubSubComponent, "video-start-processing", false)]
    async ([FromBody] TakeScreenshotRequest message, [FromServices] ITakeScreenshotService service) =>
    {
        Console.WriteLine($"[Consumer] Start processing video: {message.VideoFileName}");
        var result = await service.TakeScreenshot(message);
        return result.Succeeded is false ? Results.Problem(result.Message) : Results.Ok();
    });

if (AppConstants.TempFilePath.Exists is false)
{
    AppConstants.TempFilePath.Create();
}


app.MapHealthChecks("/health");

app.Run();

public partial class Program { }