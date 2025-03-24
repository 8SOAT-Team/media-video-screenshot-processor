using Shouldly;
using VideoScreenshot.Application.Requests;
using VideoScreenshot.Tests.Fakes;

namespace VideoScreenshot.Tests.UnitTests.Application.Requests;

public sealed class TakeScreenshotRequestTest
{
    [Fact]
    public void VideoUrl_GivenValidUrl_Should_SetValidVideoUri()
    {
        // Arrange
        var fileName = FakerBr.Faker.Random.Word();
        
        // Act
        var request = new TakeScreenshotRequest
        {
            VideoName = fileName
        };
        
        // Assert
        request.VideoFileName.ShouldNotBeNull();
        request.VideoFileName.ToString().ShouldBe(fileName);
    }
}