using VideoScreenshot.Domain.Results;

namespace VideoScreenshot.Application.Driven.Files;

public interface IPackFileService
{
    OperationResult PackSourceDirectory(DirectoryInfo sourceDirectory, FileInfo destinationFile);
}