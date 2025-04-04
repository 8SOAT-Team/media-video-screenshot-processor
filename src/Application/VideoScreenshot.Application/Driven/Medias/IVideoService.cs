using VideoScreenshot.Domain.Results;
using VideoScreenshot.Domain.ValueObjects;

namespace VideoScreenshot.Application.Driven.Medias;

public interface IVideoService
{
    /// <summary>
    /// Generate screenshots from a video file
    /// </summary>
    /// <param name="videoFile">input video stored path</param>
    /// <returns>Created files localization in original path /images</returns>
    Task<OperationResult<DirectoryInfo>> GenerateScreenshots(VideoFile videoFile);
}