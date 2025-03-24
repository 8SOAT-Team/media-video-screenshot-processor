using VideoScreenshot.Domain.Results;

namespace VideoScreenshot.Application.Driven.Medias;

public interface IVideoService
{
    /// <summary>
    /// Generate screenshots from a video file
    /// </summary>
    /// <param name="fileInfo">input video stored path</param>
    /// <returns>Created files localization in original path /images</returns>
    Task<OperationResult<DirectoryInfo>> GenerateScreenshots(FileInfo fileInfo);
    
    OperationResult<TimeSpan> GetDuration(FileInfo fileInfo);

    Task<OperationResult> TakeScreenshot(FileInfo targetFileInfo, DirectoryInfo outputPath, TimeSpan screenshotTime);
}