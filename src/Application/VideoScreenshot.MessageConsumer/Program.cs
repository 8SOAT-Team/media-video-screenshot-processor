using Dapr;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VideoScreenshot.Application.Configurations;
using VideoScreenshot.Application.Drivers;
using VideoScreenshot.Application.Requests;
using VideoScreenshot.Infrastructure.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationPorts();
builder.Services.AddInfrastructurePorts();

var app = builder.Build();

app.UseCloudEvents();

app.MapSubscribeHandler();

app.MapPost("/video-start-processing",
    [Topic("snssqs-pubsub", "video-start-processing", false)]
    async ([FromBody] TakeScreenshotRequest message, [FromServices] ITakeScreenshotService service) =>
    {
        var result = await service.TakeScreenshot(message);
        return result.Succeeded is false ? Results.Problem(result.Message) : Results.Ok();
    });

if (AppConstants.TempFilePath.Exists is false)
{
    AppConstants.TempFilePath.Create();
}

app.Run();