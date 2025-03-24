using System.Diagnostics.CodeAnalysis;
using MediaToolkit;
using Microsoft.Extensions.DependencyInjection;
using VideoScreenshot.Application.Driven.Engines;
using VideoScreenshot.Application.Drivers;

namespace VideoScreenshot.Application.Configurations;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static void AddApplicationPorts(this IServiceCollection services)
    {
        services.AddSingleton<ITakeScreenshotService, TakeScreenshotService>();
        services.AddSingleton(new Engine(@"../../Application/VideoScreenshot.Infrastructure/Components/ffmpeg.exe"));
        services.AddSingleton<IVideoEngineService, VideoEngineService>();
    }
}