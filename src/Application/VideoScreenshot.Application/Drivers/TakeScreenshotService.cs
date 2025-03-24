using VideoScreenshot.Application.Driven.Buckets;
using VideoScreenshot.Application.Driven.Files;
using VideoScreenshot.Application.Driven.Medias;
using VideoScreenshot.Application.Requests;
using VideoScreenshot.Domain.Results;

namespace VideoScreenshot.Application.Drivers;

internal sealed class TakeScreenshotService(
    IFileService fileService,
    IVideoService videoService,
    IBucketReader bucketReader,
    IBucketWriter bucketWriter) : ITakeScreenshotService
{
    private const string PackedFileName = "images.zip";

    public async Task<OperationResult> TakeScreenshot(TakeScreenshotRequest request)
    {
        var requestValid = request.Validate();
        
        if (requestValid.Succeeded is false)
        {
            return requestValid;
        }
        
        var localVideoFilePath = await bucketReader.DownloadFileToLocalStorage(request.VideoFileName, request.ProcessId);

        if (localVideoFilePath.Succeeded is false)
        {
            return OperationResult.Fail(localVideoFilePath.Message!);
        }
    
        var screenshotOutputDirectory = await videoService.GenerateScreenshots(localVideoFilePath.Value!);

        if (screenshotOutputDirectory.Succeeded is false)
        {
            return OperationResult.Fail(screenshotOutputDirectory.Message!);
        }

        var packagePath = Path.Combine(localVideoFilePath.Value!.DirectoryName!, PackedFileName);
        var packageFileInfo = new FileInfo(packagePath);
        var packedFileResult = fileService.PackFilesTo(screenshotOutputDirectory.Value!, packageFileInfo);

        if (packedFileResult.Succeeded is false)
        {
            return OperationResult.Fail(packedFileResult.Message!);
        }

        var uploadResult = await bucketWriter.UploadFileToBucket(packageFileInfo, request.ProcessId);

        return uploadResult.Succeeded
            ? OperationResult.DefaultSuccessResult
            : OperationResult.Fail(uploadResult.Message!);
    }
}