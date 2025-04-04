﻿using System.ComponentModel.DataAnnotations;

namespace VideoScreenshot.Application.Configurations;

public sealed class AppConfiguration
{
    public const string ConfigurationSectionName = "AppConfiguration";

    public const string PubSubComponent = "snssqs-pubsub";

    [Required] [MinLength(1)] public string FfmpegPath { get; init; } = null!;

    [Required] [MinLength(1)] public string BucketComponentHttpUrl { get; init; } = null!;
    [Required] [MinLength(1)] public string BucketComponentGrpcUrl { get; init; } = null!;
}