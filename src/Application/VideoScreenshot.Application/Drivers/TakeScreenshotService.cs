using VideoScreenshot.Application.Driven.Buckets;
using VideoScreenshot.Application.Driven.Files;
using VideoScreenshot.Application.Driven.Medias;
using VideoScreenshot.Application.Driven.MessageBrokers;
using VideoScreenshot.Application.Requests;
using VideoScreenshot.Domain.Events;
using VideoScreenshot.Domain.Results;
using VideoScreenshot.Domain.ValueObjects;

namespace VideoScreenshot.Application.Drivers;

internal sealed class TakeScreenshotService(
    IVideoService videoService,
    IBucketReader bucketReader,
    IBucketWriter bucketWriter,
    IEventBusService eventBus,
    IPackFileService packFileService) : ITakeScreenshotService
{
    public async Task<OperationResult> TakeScreenshot(TakeScreenshotRequest request)
    {
        var requestValid = request.Validate();

        if (requestValid.Succeeded is false)
        {
            return requestValid;
        }

        var localVideoFilePath =
            await bucketReader.DownloadFileToLocalStorage(request.VideoFileName, request.ProcessId);

        if (localVideoFilePath.Succeeded is false)
        {
            return OperationResult.Fail(localVideoFilePath.Message!);
        }

        var videoFile = new VideoFile(
            localVideoFilePath.Value!,
            request.ProcessId
        );

        var screenshotOutputDirectory = await videoService.GenerateScreenshots(videoFile);

        if (screenshotOutputDirectory.Succeeded is false)
        {
            videoFile.SetFailed(screenshotOutputDirectory.Message!);            
            await ReleaseEvents(videoFile);
            return OperationResult.Fail(screenshotOutputDirectory.Message!);
        }

        var packedFileResult = packFileService.PackSourceDirectory(videoFile.FilePackage.SourceDirectory,
            videoFile.FilePackage.DestinationFile);
        
        if (packedFileResult.Succeeded is false)
        {
            videoFile.SetFailed(packedFileResult.Message!);
            await ReleaseEvents(videoFile);
            return OperationResult.Fail(packedFileResult.Message!);
        }

        var uploadResult = await bucketWriter.UploadFileToBucket(videoFile.FilePackage.DestinationFile, request.ProcessId);

        if (uploadResult.Succeeded) return OperationResult.DefaultSuccessResult;
        
        videoFile.SetFailed(uploadResult.Message!);
        await ReleaseEvents(videoFile);
        return OperationResult.Fail(uploadResult.Message!);
    }

    private async Task ReleaseEvents(VideoFile videoFile)
    {
        foreach (var @event in videoFile.ReleaseEvents())
        {
            await eventBus.Publish(@event);
        }
    }
}