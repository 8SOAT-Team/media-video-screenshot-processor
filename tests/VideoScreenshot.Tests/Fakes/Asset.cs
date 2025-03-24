using System.Reflection;

namespace VideoScreenshot.Tests.Fakes;

public static class Asset
{
    public static FileInfo VideoFile = new (Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "../../../..", "assets", "fiap.mp4"));
}