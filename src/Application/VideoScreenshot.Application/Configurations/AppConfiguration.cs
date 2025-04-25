using System.ComponentModel.DataAnnotations;

namespace VideoScreenshot.Application.Configurations;

public sealed class AppConfiguration
{
    public const string ConfigurationSectionName = "AppConfiguration";

    public const string PubSubComponent = "snssqs-pubsub";

    [Required] [MinLength(1)] public string FfmpegPath { get; init; } = null!;

    [Required] [MinLength(1)] public string DaprHttpUrl { get; init; } = null!;
    [Required] [MinLength(1)] public string DaprGrpcUrl { get; init; } = null!;
    
    public string QueueUrl { get; init; } = null!;
    public int MaxMessages { get; set; }
    public int LongPollTimeSeconds { get; set; }
    
    public string S3BucketName { get; init; } = null!;
    public string PublishEventQueueUrl { get; init; } = null!;
}