using VideoScreenshot.Domain.Results;
using VideoScreenshot.Domain.ValueObjects;

namespace VideoScreenshot.Application.Driven.Buckets;

public interface IBucketReader
{
    Task<OperationResult<FileInfo>> DownloadFileToLocalStorage(FileName fileName, Guid processId);
}