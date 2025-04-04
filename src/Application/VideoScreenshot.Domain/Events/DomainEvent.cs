namespace VideoScreenshot.Domain.Events;

public record DomainEvent
{
    public DateTime CreatedAt { get; private init; } = DateTime.UtcNow;
}