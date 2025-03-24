using VideoScreenshot.Application.Driven.Engines;
using VideoScreenshot.Application.Driven.Medias;
using VideoScreenshot.Domain.Results;

namespace VideoScreenshot.Infrastructure.Driven.Medias;

internal sealed class VideoService(IVideoEngineService engine) : IVideoService
{
    /// <inheritdoc cref="IVideoService"/>
    public async Task<OperationResult<DirectoryInfo>> GenerateScreenshots(FileInfo fileInfo)
    {
        await Task.Yield();

        var outputFolder = Path.Combine(fileInfo.DirectoryName!, "images");
        var outputDirectory = Directory.CreateDirectory(outputFolder);
        var duration = GetDuration(fileInfo);

        if (duration.Succeeded is false)
        {
            return OperationResult<DirectoryInfo>.Fail("Failed to get video duration");
        }

        var interval = TimeSpan.FromSeconds(1);

        for (var currentTime = TimeSpan.Zero; currentTime < duration.Value; currentTime += interval)
        {
            var result = await TakeScreenshot(fileInfo, outputDirectory, currentTime);

            if (result.Succeeded) continue;

            Directory.Delete(outputFolder, true);
            return OperationResult<DirectoryInfo>.Fail($"Failed to take screenshot at {currentTime}");
        }

        return OperationResult<DirectoryInfo>.Success(outputDirectory, "Screenshots generated");
    }

    public OperationResult<TimeSpan> GetDuration(FileInfo fileInfo)
    {
        if (fileInfo.Exists is false)
        {
            return OperationResult<TimeSpan>.Fail("File does not exist");
        }

        var mediaFile = engine.LoadMetadata(fileInfo);

        return OperationResult<TimeSpan>.Success(mediaFile.Duration, "Duration retrieved");
    }

    public async Task<OperationResult> TakeScreenshot(FileInfo targetFileInfo, DirectoryInfo outputPath,
        TimeSpan screenshotTime)
    {
        if (targetFileInfo.Exists is false)
        {
            return OperationResult.Fail("File does not exist");
        }

        var outputFile = Path.Combine(outputPath.FullName, $"frame_at_{screenshotTime.Seconds}.jpg");

        await engine.GetThumbnail(targetFileInfo, new FileInfo(outputFile), screenshotTime);

        return OperationResult.DefaultSuccessResult;
    }
}