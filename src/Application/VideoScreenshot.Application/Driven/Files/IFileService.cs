using VideoScreenshot.Domain.Results;

namespace VideoScreenshot.Application.Driven.Files;

public interface IFileService
{
    OperationResult PackFilesTo(DirectoryInfo directory, FileInfo destinationFile);
}