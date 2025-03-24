namespace VideoScreenshot.Infrastructure.Configurations;

public static class AppConstants
{
    public const string S3BucketBindingName = "s3bucket";
    public static readonly DirectoryInfo TempFilePath = new(Path.Combine(Path.GetTempPath(), "VideoScreenshot"));
}