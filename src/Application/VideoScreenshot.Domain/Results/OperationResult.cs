namespace VideoScreenshot.Domain.Results;

public sealed record OperationResult<T>(bool Succeeded, T? Value, string? Message)
{
    public static OperationResult<T> Success(T value, string? message) => new(true, value, message);
    public static OperationResult<T> Fail(string message) => new(false, default, message);
}

public sealed record OperationResult(bool Succeeded, string? Message)
{
    public static readonly OperationResult DefaultSuccessResult = new(true,null!);
    
    public static OperationResult Success(string? message) => new(true, message);
    public static OperationResult Fail(string message) => new(false, message);
}