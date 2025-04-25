using VideoScreenshot.Application.Configurations;
using VideoScreenshot.Application.Driven.Buckets;
using VideoScreenshot.Domain.Results;
using VideoScreenshot.Domain.ValueObjects;
using VideoScreenshot.Infrastructure.Configurations;
using VideoScreenshot.Infrastructure.Driven.Buckets.Clients;

namespace VideoScreenshot.Infrastructure.Driven.Buckets;

public class BucketReader(IBucketClientProxy client, AppConfiguration appConfig) : IBucketReader
{
    public async Task<OperationResult<FileInfo>> DownloadFileToLocalStorage(FileName fileName, Guid processId)
    {
        var processDirectory = Path.Combine(AppConstants.TempFilePath.FullName, processId.ToString());
        if (Directory.Exists(processDirectory) is false)
        {
            Directory.CreateDirectory(processDirectory);
        }
        else
        {
            Directory.Delete(processDirectory, true);
            Directory.CreateDirectory(processDirectory);
        }

        var writeTo = Path.Combine(processDirectory, fileName.Name);
        await client.DownloadToFilePathAsync(appConfig.S3BucketName, $"{processId.ToString().ToUpper()}/{fileName}",
            writeTo);

        return OperationResult<FileInfo>.Success(new FileInfo(writeTo), "Download realizado com sucesso");
    }
}