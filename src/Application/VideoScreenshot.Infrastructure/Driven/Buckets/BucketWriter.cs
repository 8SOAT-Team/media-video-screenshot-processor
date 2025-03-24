using Dapr.Client;
using VideoScreenshot.Application.Driven.Buckets;
using VideoScreenshot.Domain.Results;
using VideoScreenshot.Infrastructure.Configurations;
using VideoScreenshot.Infrastructure.Driven.Buckets.Clients;

namespace VideoScreenshot.Infrastructure.Driven.Buckets;

public class BucketWriter(IBucketClientProxy client) : IBucketWriter
{
    public async Task<OperationResult> UploadFileToBucket(FileInfo file, Guid processId)
    {
        var fileContent = await File.ReadAllBytesAsync(file.FullName);

        var request = new BindingRequest(AppConstants.S3BucketBindingName, "create")
        {
            Data = fileContent,

            Metadata =
            {
                { "key", $"{processId.ToString().ToUpper()}/{file.Name}" },
            }
        };

        await client.InvokeBindingAsync(request);

        return OperationResult.Success("Upload realizado com sucesso");
    }
}