namespace VehicleX.Application.Common;

public class ServiceResult<T>
{
    public bool Success { get; private init; }
    public int StatusCode { get; private init; }
    public string Message { get; private init; } = string.Empty;
    public T? Data { get; private init; }
    public IReadOnlyDictionary<string, string[]>? Errors { get; private init; }

    public static ServiceResult<T> Ok(T? data, string message = "Request completed successfully.", int statusCode = 200)
    {
        return new ServiceResult<T>
        {
            Success = true,
            StatusCode = statusCode,
            Message = message,
            Data = data
        };
    }

    public static ServiceResult<T> Fail(string message, int statusCode, IReadOnlyDictionary<string, string[]>? errors = null)
    {
        return new ServiceResult<T>
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Errors = errors
        };
    }
}
