using System.Diagnostics.CodeAnalysis;

namespace VideoScreenshot.Domain.ValueObjects;

[ExcludeFromCodeCoverage]
public class MediaFile
{
    public string Path { get; init; } = null!;
    public TimeSpan Duration { get; set; }
}