using System.Text.Json;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDaprClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/video/start-processing",
    async ([FromBody] StartVideoProcessingMessage request, [FromServices] DaprClient client) =>
    {
        Console.WriteLine($"[Publisher] Start processing video: {request.VideoName}");
        await client.PublishEventAsync("snssqs-pubsub", "video-start-processing", request);
    });

app.Run();

public record StartVideoProcessingMessage(Guid ProcessId, string VideoName);