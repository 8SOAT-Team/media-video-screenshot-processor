using VideoScreenshot.Domain.ValueObjects;
using ToolKitMediaFile = MediaToolkit.Model.MediaFile;

namespace VideoScreenshot.Application.Adapters;

public static class MediaFileAdapter
{
    public static MediaFile ToMediaFile(this ToolKitMediaFile toolKitMediaFile)
    {
        return new MediaFile
        {
            Path = toolKitMediaFile.Filename,
            Duration = toolKitMediaFile.Metadata?.Duration ?? TimeSpan.Zero,
        };
    }
}