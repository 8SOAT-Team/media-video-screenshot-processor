using VideoScreenshot.Domain.Results;

namespace VideoScreenshot.Infrastructure.Driven.Buckets.Clients;

public interface IBucketClientProxy
{
    Task DownloadToFilePathAsync(string bucketName, string key, string destinationPath);
    Task<OperationResult> UploadFileAsync(string bucketName, string key, string filePath, string contentType);
}