using System.Net.Http.Json;
using VideoScreenshot.Application.Requests;
using VideoScreenshot.Tests.Fakes;
using VideoScreenshot.Tests.Integration.Applications;

namespace VideoScreenshot.Tests.Integration.MessageConsumerTests;

public class VideoStartProcessingTest : IClassFixture<ConsumerWebApplicationFactory>
{
    private readonly ConsumerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public VideoStartProcessingTest(ConsumerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    
    [Fact(Skip = "Skip until fix dapr integration tests")]
    public async Task ShouldStartProcessingVideo()
    {
        // Arrange
        var processId = Guid.NewGuid();
        var fileName = Asset.VideoFile.VolumeFile.Name;

        // Act
        var response = await _client.PostAsJsonAsync("/video-start-processing", new
        {
            ProcessId = processId,
            VideoName = fileName
        });

        // Assert
        response.EnsureSuccessStatusCode();
    }
}