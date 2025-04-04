using System.Reflection;
using VideoScreenshot.Domain.ValueObjects;

namespace VideoScreenshot.Tests.Fakes;

public static class Asset
{
    public static VideoFile VideoFile = new(
        new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "../../../..",
            "assets", "fiap.mp4")),
        Guid.Empty);
}