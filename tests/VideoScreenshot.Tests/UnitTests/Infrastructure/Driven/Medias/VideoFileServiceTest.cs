using NSubstitute;
using Shouldly;
using VideoScreenshot.Application.Driven.Engines;
using VideoScreenshot.Domain.ValueObjects;
using VideoScreenshot.Infrastructure.Driven.Medias;
using VideoScreenshot.Tests.Fakes;

namespace VideoScreenshot.Tests.UnitTests.Infrastructure.Driven.Medias;

public sealed class VideoFileServiceTest
{
    private readonly IVideoEngineService _videoEngineService;
    private readonly VideoService _videoService;

    public VideoFileServiceTest()
    {
        _videoEngineService = Substitute.For<IVideoEngineService>();
        _videoService = new VideoService(_videoEngineService);
    }

    [Fact]
    public void GetDuration_Given_FileDoesNotExists_Should_ReturnFailure()
    {
        // Arrange
        var fileInfo = new FileInfo(FakerBr.Faker.System.FilePath());

        //Act
        var result = _videoService.GetDuration(fileInfo);
        
        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Message.ShouldBe("File does not exist");
    }

    [Fact]
    public void GetDuration_Given_FileExists_Should_ReturnDuration()
    {
        // Arrange
        var fileInfo = Asset.VideoFile;
        var mediaFile = new MediaFile
        {
            Duration = TimeSpan.FromSeconds(FakerBr.Faker.Random.Number(1, 60)),
            Path = fileInfo.VolumeFile.FullName
        };

        _videoEngineService.LoadMetadata(Arg.Any<FileInfo>()).Returns(mediaFile);

        // Act
        var result = _videoService.GetDuration(fileInfo.VolumeFile);
        
        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Message.ShouldBe("Duration retrieved");
        result.Value.ShouldBeGreaterThanOrEqualTo(mediaFile.Duration);
    }

    [Fact]
    public async Task GenerateScreenshots_Given_VideoFile_Should_ReturnSuccessAfterCallMethods()
    {
        // Arrange
        var fileInfo = Asset.VideoFile;
        var mediaFile = new MediaFile
        {
            Duration = TimeSpan.FromSeconds(FakerBr.Faker.Random.Number(1, 60)),
            Path = fileInfo.VolumeFile.FullName
        };

        _videoEngineService.LoadMetadata(Arg.Any<FileInfo>()).Returns(mediaFile);

        // Act
        var result = await _videoService.GenerateScreenshots(fileInfo);
        
        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Message.ShouldBe("Screenshots generated");
        
        _videoEngineService.Received(1).LoadMetadata(Arg.Any<FileInfo>());
        await _videoEngineService.Received()
            .GetThumbnail(Arg.Any<FileInfo>(), Arg.Any<FileInfo>(), Arg.Any<TimeSpan>());
    }
}