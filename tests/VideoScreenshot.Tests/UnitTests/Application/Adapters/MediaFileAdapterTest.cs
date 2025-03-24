using Bogus;
using Shouldly;
using VideoScreenshot.Application.Adapters;
using ToolKitMediaFile = MediaToolkit.Model.MediaFile;

namespace VideoScreenshot.Tests.UnitTests.Application.Adapters;

public sealed class MediaFileAdapterTest
{
   
    [Fact]
    public void MediaFileAdapter_Given_ToolKitMediaFile_Should_Return_MediaFile()
    {
        // Arrange
        var toolKitMediaFile = new Faker<ToolKitMediaFile>()
            .RuleFor(x => x.Filename, f => f.System.FileName())
            .Generate();
        
        // Act
        var mediaFile = toolKitMediaFile.ToMediaFile();
        
        // Assert
        mediaFile.ShouldNotBeNull();
        mediaFile.Path.ShouldBe(toolKitMediaFile.Filename);
    }
}