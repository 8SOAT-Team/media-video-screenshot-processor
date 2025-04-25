using Amazon.S3;
using Amazon.S3.Model;
using VideoScreenshot.Domain.Results;
using VideoScreenshot.Infrastructure.Driven.Buckets.Clients;

namespace VideoScreenshot.Infrastructure.Driven.Buckets;

public sealed class BucketClientProxy(IAmazonS3 amazonS3) : IBucketClientProxy
{
    public async Task DownloadToFilePathAsync(string bucketName, string key, string destinationPath)
    {
       await amazonS3.DownloadToFilePathAsync(bucketName, key, destinationPath, null);
    }

    public async Task<OperationResult> UploadFileAsync(string bucketName, string key, string filePath, string contentType)
    {
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            FilePath = filePath,
            ContentType = contentType
        };

        var response = await amazonS3.PutObjectAsync(request);

        return ((int)response.HttpStatusCode) >= 200 && ((int)response.HttpStatusCode) < 300
            ? OperationResult.Success("Upload realizado com sucesso")
            : OperationResult.Fail("Falha ao realizar o upload");
    }
}
