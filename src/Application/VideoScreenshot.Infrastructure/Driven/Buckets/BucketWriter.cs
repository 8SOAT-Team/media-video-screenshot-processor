using VideoScreenshot.Application.Configurations;
using VideoScreenshot.Application.Driven.Buckets;
using VideoScreenshot.Domain.Results;
using VideoScreenshot.Infrastructure.Driven.Buckets.Clients;

namespace VideoScreenshot.Infrastructure.Driven.Buckets;

public class BucketWriter(IBucketClientProxy client, AppConfiguration appConfig) : IBucketWriter
{
    public async Task<OperationResult> UploadFileToBucket(FileInfo file, Guid processId)
    {
        var uploadResult = await client.UploadFileAsync(appConfig.S3BucketName,
            $"{processId.ToString().ToUpper()}/{file.Name}",
            file.FullName,
            "application/zip");

        return uploadResult.Succeeded
            ? OperationResult.Success("Upload realizado com sucesso")
            : uploadResult;
    }
}