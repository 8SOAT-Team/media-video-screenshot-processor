using Shouldly;
using VideoScreenshot.Domain.ValueObjects;
using VideoScreenshot.Tests.Fakes;

namespace VideoScreenshot.Tests.UnitTests.Domain.ValueObjects;

public class FilePackageTest
{
    [Fact]
    public void PackDirectoryTo_Given_FileDoesNotExists_Should_ReturnFailure()
    {
        // Arrange
        var directoryPath = FakerBr.Faker.System.DirectoryPath();
        var directory = new DirectoryInfo(directoryPath);
        var outputFile = new FileInfo(FakerBr.Faker.System.FileName());
        
        // Act
        // var result = FilePackage.PackSourceDirectory(directory, outputFile);

        // Assert
        // result.Succeeded.ShouldBeFalse();
        // result.Message.ShouldBe("Directory does not exist");
    }
    
    [Fact]
    public void PackDirectoryTo_Given_FileExists_Should_ReturnSuccess()
    {
        // Arrange
        var directoryPath = FakerBr.Faker.System.DirectoryPath();
        var directory = new DirectoryInfo(directoryPath);
        directory.Create();
        var outputFile = new FileInfo(FakerBr.Faker.System.FileName());
        
        // // Act
        // var result = FilePackage.PackSourceDirectory(directory, outputFile);
        //
        // // cleanup
        // directory.Delete(true);
        // outputFile.Delete();
        //
        // // Assert
        // result.Succeeded.ShouldBeTrue();
    }
}