using System.Diagnostics.CodeAnalysis;
using MediaToolkit;
using Microsoft.Extensions.DependencyInjection;
using VideoScreenshot.Application.Driven.Engines;
using VideoScreenshot.Application.Drivers;

namespace VideoScreenshot.Application.Configurations;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static void AddApplicationPorts(this IServiceCollection services, string ffmpegPath)
    {
        services.AddSingleton<ITakeScreenshotService, TakeScreenshotService>();
        services.AddSingleton<Engine>(_ => new Engine(ffmpegPath));
        services.AddSingleton<IVideoEngineService, VideoEngineService>();
    }
}