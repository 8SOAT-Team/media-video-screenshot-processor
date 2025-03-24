using VideoScreenshot.Domain.Results;

namespace VideoScreenshot.Application.Driven.Buckets;

public interface IBucketWriter
{
    Task<OperationResult> UploadFileToBucket(FileInfo file, Guid processId);
}