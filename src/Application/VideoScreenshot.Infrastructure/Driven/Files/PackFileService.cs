﻿using System.IO.Compression;
using VideoScreenshot.Application.Driven.Files;
using VideoScreenshot.Domain.Results;

namespace VideoScreenshot.Infrastructure.Driven.Files;

public class PackFileService : IPackFileService
{
    public OperationResult PackSourceDirectory(DirectoryInfo sourceDirectory, FileInfo destinationFile)
    {
        if (!sourceDirectory.Exists)
            return OperationResult.Fail("Directory does not exist");

        if (destinationFile.Exists)
        {
            destinationFile.Delete();
        }

        try
        {
            ZipFile.CreateFromDirectory(sourceDirectory.FullName, destinationFile.FullName);
        }
        catch (Exception e)
        {
            return OperationResult.Fail(e.Message);
        }

        return OperationResult.DefaultSuccessResult;
    }
}