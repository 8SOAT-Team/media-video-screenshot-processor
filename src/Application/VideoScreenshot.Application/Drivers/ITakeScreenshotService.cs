using VideoScreenshot.Application.Requests;
using VideoScreenshot.Domain.Results;

namespace VideoScreenshot.Application.Drivers;

public interface ITakeScreenshotService
{
    Task<OperationResult> TakeScreenshot(TakeScreenshotRequest request);
}