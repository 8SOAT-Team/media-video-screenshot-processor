namespace VideoScreenshot.Domain.ValueObjects;

public class FilePackage
{
    public required DirectoryInfo SourceDirectory { get; init; }
    public required FileInfo DestinationFile { get; init; }
}