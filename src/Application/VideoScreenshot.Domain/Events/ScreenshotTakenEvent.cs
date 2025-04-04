namespace VideoScreenshot.Domain.Events;

public record ScreenshotTakenEvent(Guid RequestProcessId, bool UploadResultSucceeded, string? UploadResultMessage) : DomainEvent;
