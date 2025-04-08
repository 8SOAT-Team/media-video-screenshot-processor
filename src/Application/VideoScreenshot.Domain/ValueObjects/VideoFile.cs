using VideoScreenshot.Domain.Events;
using VideoScreenshot.Domain.Types;

namespace VideoScreenshot.Domain.ValueObjects;

public class VideoFile
{
    private HashSet<DomainEvent> _domainEvents = [];
    private const string PackedFileName = "images.zip";

    public VideoFile(FileInfo volumeFile, Guid relatedProcess)
    {
        VolumeFile = volumeFile;
        RelatedProcess = relatedProcess;
        FilePackage = new FilePackage
        {
            SourceDirectory = new DirectoryInfo(ThumbnailPath),
            DestinationFile = new FileInfo(Path.Combine(VolumeFile.DirectoryName!, PackedFileName)),
        };
        Status = VideoFileProcessingStatus.Created;
    }

    public FileInfo VolumeFile { get; init; }
    public Guid RelatedProcess { get; init; }

    public VideoFileProcessingStatus Status { get; private set; }

    public string ThumbnailPath => Path.Combine(VolumeFile.DirectoryName!, "images");

    public FilePackage FilePackage { get; private init; }

    private TimeSpan? Duration { get; set; }

    public void SetDuration(TimeSpan duration)
    {
        Duration = duration;
        Status = VideoFileProcessingStatus.Processing;
    }

    public void SetUploaded()
    {
        Status = VideoFileProcessingStatus.Processed;
        _domainEvents.Add(new ScreenshotTakenEvent(RelatedProcess, true, "Screenshots uploaded"));
    }

    public void SetFailed(string message)
    {
        Status = VideoFileProcessingStatus.Processed;
        _domainEvents.Add(new ScreenshotTakenEvent(RelatedProcess, false, message));
    }

    public TimeSpan GetInterval()
    {
        if (Duration is null)
        {
            throw new ApplicationException("Duration is not set");
        }

        return Duration.Value.TotalMilliseconds switch
        {
            >= 120_000 => TimeSpan.FromSeconds(20),
            >= 60_000 => TimeSpan.FromSeconds(10),
            >= 20_000 => TimeSpan.FromSeconds(5),
            >= 10_000 => TimeSpan.FromSeconds(2),
            <= 1_000 => TimeSpan.FromMilliseconds(500),
            _ => TimeSpan.FromSeconds(1)
        };
    }

    public DomainEvent[] ReleaseEvents()
    {
        var events = new DomainEvent[_domainEvents.Count];
        _domainEvents.CopyTo(events);
        
        _domainEvents.Clear();
        return events;
    }

    public static readonly TemplateString ThumbnailOutputNameTemplate = "frame_at_{0}.jpg";
}