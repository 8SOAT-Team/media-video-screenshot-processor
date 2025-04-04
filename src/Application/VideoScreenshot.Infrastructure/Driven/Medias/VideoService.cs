using VideoScreenshot.Application.Driven.Engines;
using VideoScreenshot.Application.Driven.Medias;
using VideoScreenshot.Domain.Results;
using VideoScreenshot.Domain.ValueObjects;

namespace VideoScreenshot.Infrastructure.Driven.Medias;

internal sealed class VideoService(IVideoEngineService engine) : IVideoService
{
    /// <inheritdoc cref="IVideoService"/>
    public async Task<OperationResult<DirectoryInfo>> GenerateScreenshots(VideoFile videoFile)
    {
        await Task.Yield();
        
        var outputDirectory = Directory.CreateDirectory(videoFile.ThumbnailPath);
        var duration = GetDuration(videoFile.VolumeFile);

        if (duration.Succeeded is false)
        {
            return OperationResult<DirectoryInfo>.Fail("Failed to get video duration");
        }

        videoFile.SetDuration(duration.Value);
        var interval = videoFile.GetInterval();
        
        for (var currentTime = TimeSpan.Zero; currentTime < duration.Value; currentTime += interval)
        {
            var result = await TakeScreenshot(videoFile.VolumeFile, outputDirectory, currentTime);

            if (result.Succeeded) continue;

            Directory.Delete(outputDirectory.FullName, true);
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

    private async Task<OperationResult> TakeScreenshot(FileInfo targetFileInfo, DirectoryInfo outputPath,
        TimeSpan screenshotTime)
    {
        var outputFile = Path.Combine(outputPath.FullName, VideoFile.ThumbnailOutputNameTemplate.Format(screenshotTime));

        await engine.GetThumbnail(targetFileInfo, new FileInfo(outputFile), screenshotTime);

        return OperationResult.DefaultSuccessResult;
    }
}