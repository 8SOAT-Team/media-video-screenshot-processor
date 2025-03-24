using VideoScreenshot.Domain.Results;
using VideoScreenshot.Domain.ValueObjects;

namespace VideoScreenshot.Application.Requests;

public sealed record TakeScreenshotRequest
{
    private readonly FileName? _videoName;

    public string VideoName
    {
        init => _videoName = new FileName(value);
    }

    public Guid ProcessId { get; init; }
    public FileName VideoFileName => _videoName ?? throw new InvalidOperationException("Video Name is not valid.");

    public OperationResult Validate()
    {
        return ProcessId.Equals(Guid.Empty)
            ? OperationResult.Fail("Process ID is required.")
            : OperationResult.Success(null);
    }
}