using VideoScreenshot.Domain.ValueObjects;

namespace VideoScreenshot.Application.Driven.Engines;

public interface IVideoEngineService
{
    MediaFile LoadMetadata(FileInfo inputFile);
    Task GetThumbnail(FileInfo inputFile, FileInfo outputFile, TimeSpan screenshotTime);
}