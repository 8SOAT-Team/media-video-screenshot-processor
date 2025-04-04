using NSubstitute;
using Shouldly;
using VideoScreenshot.Application.Driven.Buckets;
using VideoScreenshot.Application.Driven.Files;
using VideoScreenshot.Application.Driven.Medias;
using VideoScreenshot.Application.Driven.MessageBrokers;
using VideoScreenshot.Application.Drivers;
using VideoScreenshot.Application.Requests;
using VideoScreenshot.Domain.Results;
using VideoScreenshot.Domain.ValueObjects;
using VideoScreenshot.Tests.Fakes;

namespace VideoScreenshot.Tests.UnitTests.Application.Drivers;

public sealed class TakeScreenshotServiceTest
{
    private readonly TakeScreenshotService _sut;
    private readonly IVideoService _videoService;
    private readonly IBucketReader _bucketReader;
    private readonly IBucketWriter _bucketWriter;
    private readonly IEventBusService _eventBus;
    private readonly IPackFileService _packFileService;

    public TakeScreenshotServiceTest()
    {
        _videoService = Substitute.For<IVideoService>();
        _bucketReader = Substitute.For<IBucketReader>();
        _bucketWriter = Substitute.For<IBucketWriter>();
        _eventBus = Substitute.For<IEventBusService>();
        _packFileService = Substitute.For<IPackFileService>();

        _sut = new TakeScreenshotService(_videoService, _bucketReader, _bucketWriter, _eventBus, _packFileService);
    }

    [Fact]
    public async Task TakeScreenshot_Given_Request_VideoUriNotFound_Should_ReturnFail()
    {
        // Arrange
        var request = new TakeScreenshotRequest
        {
            VideoName = FakerBr.Faker.Internet.UrlWithPath(),
            ProcessId = FakerBr.Faker.Random.Guid()
        };

        _bucketReader.DownloadFileToLocalStorage(Arg.Any<FileName>(), Arg.Any<Guid>())
            .Returns(OperationResult<FileInfo>.Fail("File not found"));

        // Act
        var result = await _sut.TakeScreenshot(request);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Message.ShouldBe("File not found");
    }

    [Fact]
    public async Task TakeScreenshot_Given_GenerateScreenshotsReturnsFail_Should_ReturnFailure()
    {
        // Arrange
        var request = new TakeScreenshotRequest
        {
            VideoName = FakerBr.Faker.Internet.UrlWithPath(),
            ProcessId = FakerBr.Faker.Random.Guid()
        };

        var fileInfo = new FileInfo(FakerBr.Faker.System.FilePath());
        _bucketReader.DownloadFileToLocalStorage(Arg.Any<FileName>(), Arg.Any<Guid>())
            .Returns(OperationResult<FileInfo>.Success(fileInfo, "File downloaded"));

        _videoService.GenerateScreenshots(Arg.Any<VideoFile>())
            .Returns(OperationResult<DirectoryInfo>.Fail("Failed to generate screenshots"));

        // Act
        var result = await _sut.TakeScreenshot(request);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Message.ShouldBe("Failed to generate screenshots");
    }

    [Fact]
    public async Task TakeScreenshot_Given_PackFilesToReturnsFail_Should_ReturnFailure()
    {
        // Arrange
        var request = new TakeScreenshotRequest
        {
            VideoName = FakerBr.Faker.Internet.UrlWithPath(),
            ProcessId = FakerBr.Faker.Random.Guid()
        };

        var fileInfo = new FileInfo(FakerBr.Faker.System.FilePath());
        _bucketReader.DownloadFileToLocalStorage(Arg.Any<FileName>(), Arg.Any<Guid>())
            .Returns(OperationResult<FileInfo>.Success(fileInfo, "File downloaded"));

        _videoService.GenerateScreenshots(Arg.Any<VideoFile>())
            .Returns(OperationResult<DirectoryInfo>.Success(new DirectoryInfo(FakerBr.Faker.System.DirectoryPath()),
                "Screenshots generated"));

        _packFileService.PackSourceDirectory(Arg.Any<DirectoryInfo>(), Arg.Any<FileInfo>())
            .Returns(OperationResult.Fail("Failed to pack files"));
        
        // Act
        var result = await _sut.TakeScreenshot(request);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Message.ShouldBe("Failed to pack files");
    }

    [Fact]
    public async Task TakeScreenshot_Given_UploadFileToBucketReturnsFail_Should_ReturnFailure()
    {
        // Arrange
        var request = new TakeScreenshotRequest
        {
            VideoName = FakerBr.Faker.Internet.UrlWithPath(),
            ProcessId = FakerBr.Faker.Random.Guid()
        };

        var fileInfo = new FileInfo(FakerBr.Faker.System.FilePath());
        _bucketReader.DownloadFileToLocalStorage(Arg.Any<FileName>(), Arg.Any<Guid>())
            .Returns(OperationResult<FileInfo>.Success(fileInfo, "File downloaded"));

        _videoService.GenerateScreenshots(Arg.Any<VideoFile>())
            .Returns(OperationResult<DirectoryInfo>.Success(new DirectoryInfo(FakerBr.Faker.System.DirectoryPath()),
                "Screenshots generated"));
        
        _bucketWriter.UploadFileToBucket(Arg.Any<FileInfo>(), Arg.Any<Guid>())
            .Returns(OperationResult.Fail("Failed to upload file"));

        _packFileService.PackSourceDirectory(Arg.Any<DirectoryInfo>(), Arg.Any<FileInfo>())
            .Returns(OperationResult.DefaultSuccessResult);
        
        // Act
        var result = await _sut.TakeScreenshot(request);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Message.ShouldBe("Failed to upload file");
    }

    [Fact]
    public async Task
        TakeScreenshot_Given_DownloadedFile_TookScreenshots_And_UploadedToBucket_Successfully_Should_ReturnSucceed()
    {
        // Arrange
        var request = new TakeScreenshotRequest
        {
            VideoName = FakerBr.Faker.Internet.UrlWithPath(),
            ProcessId = FakerBr.Faker.Random.Guid()
        };

        var fileInfo = new FileInfo(FakerBr.Faker.System.FilePath());
        _bucketReader.DownloadFileToLocalStorage(Arg.Any<FileName>(), Arg.Any<Guid>())
            .Returns(OperationResult<FileInfo>.Success(fileInfo, "File downloaded"));

        var directoryInfo = new DirectoryInfo(FakerBr.Faker.System.DirectoryPath());
        _videoService.GenerateScreenshots(Arg.Any<VideoFile>())
            .Returns(OperationResult<DirectoryInfo>.Success(directoryInfo, "Screenshots generated"));

        _bucketWriter.UploadFileToBucket(Arg.Any<FileInfo>(), Arg.Any<Guid>())
            .Returns(OperationResult.Success("File uploaded"));

        _packFileService.PackSourceDirectory(Arg.Any<DirectoryInfo>(), Arg.Any<FileInfo>())
            .Returns(OperationResult.DefaultSuccessResult);
        
        // Act
        var result = await _sut.TakeScreenshot(request);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.ShouldBe(OperationResult.DefaultSuccessResult);
    }
}