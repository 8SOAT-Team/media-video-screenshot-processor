using MediaToolkit;
using MediaToolkit.Options;
using VideoScreenshot.Application.Adapters;
using VideoScreenshot.Domain.ValueObjects;
using ToolKitMediaFile = MediaToolkit.Model.MediaFile;

namespace VideoScreenshot.Application.Driven.Engines;

public sealed class VideoEngineService(Engine engine) : IVideoEngineService
{
    public MediaFile LoadMetadata(FileInfo inputFile)
    {
        var toolkitMediaFile = new ToolKitMediaFile(inputFile.FullName);
        engine.GetMetadata(toolkitMediaFile);

        return toolkitMediaFile.ToMediaFile();
    }

    public async Task GetThumbnail(FileInfo inputFile, FileInfo outputFile, TimeSpan screenshotTime)
    {
        await Task.Yield();
        
        engine.GetThumbnail(new ToolKitMediaFile(inputFile.FullName), new ToolKitMediaFile(outputFile.FullName),
            new ConversionOptions
            {
                Seek = screenshotTime
            });
    }
}