using Dapr.Client;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using VideoScreenshot.Application.Configurations;
using VideoScreenshot.Application.Driven.Buckets;
using VideoScreenshot.Infrastructure.Configurations;
using VideoScreenshot.Tests.Fakes;
using VideoScreenshot.Tests.Integration.Applications;
using Wrapr;
using Xunit.Abstractions;

namespace VideoScreenshot.Tests.Integration.Buckets;

public class BucketReaderTest : IClassFixture<ConsumerWebApplicationFactory>
{
    private readonly ConsumerWebApplicationFactory _factory;
    private readonly IBucketReader _bucketReader;
    private readonly ITestOutputHelper _output;
    private readonly AppConfiguration _configuration;

    private readonly int DaprHttpPort = 3073;
    private readonly int DaprGrpcPort = 3074;
    private const int AppPort = 5148;

    public BucketReaderTest(ConsumerWebApplicationFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _output = output;
        _bucketReader = _factory.Services.GetRequiredService<IBucketReader>();
        _configuration = factory.Services.GetRequiredService<AppConfiguration>();
    }

    [Fact(Skip = "Skip until fix dapr integration tests")]
    public async Task ShouldReadFileFromBucket()
    {
        // Arrange
        var processId = Guid.NewGuid();
        await using var sidecar = await Sidecar();
        using var client = new DaprClientBuilder()
            .UseGrpcEndpoint(_configuration.BucketComponentGrpcUrl)
            .UseHttpEndpoint(_configuration.BucketComponentHttpUrl)
            .Build();

        var fileContent = await File.ReadAllBytesAsync(Asset.VideoFile.VolumeFile.FullName);

        var request = new BindingRequest(AppConstants.S3BucketBindingName, "create")
        {
            Data = fileContent,
            Metadata =
            {
                { "key", $"{processId.ToString().ToUpper()}/{Asset.VideoFile.VolumeFile.Name}" },
            }
        };

        await client.InvokeBindingAsync(request);

        // Act
        var result = await _bucketReader.DownloadFileToLocalStorage(Asset.VideoFile.VolumeFile.Name, processId);

        // Assert
        result.Succeeded.ShouldBeTrue();
    }

    private async Task<Sidecar> Sidecar()
    {
        var sidecar = new Sidecar("processing-video-consumer");
        await sidecar.Start(with => with
            .ResourcesPath("Integration/Components")
            .AppPort(AppPort)
            .DaprHttpPort(DaprHttpPort)
            .DaprGrpcPort(DaprGrpcPort)
            .Args("--log-level", "debug"));

        return sidecar;
    }
}