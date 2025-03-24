using System.IO.Compression;
using VideoScreenshot.Domain.Results;

namespace VideoScreenshot.Domain.ValueObjects;

public static class FilePackage
{
    public static OperationResult PackDirectoryTo(DirectoryInfo directory, FileInfo destinationFile)
    {
        if (!directory.Exists)
            return OperationResult.Fail("Directory does not exist");

        if (destinationFile.Exists)
        {
            destinationFile.Delete();
        }
        
        ZipFile.CreateFromDirectory(directory.FullName, destinationFile.FullName);

        return OperationResult.DefaultSuccessResult;
    }
}