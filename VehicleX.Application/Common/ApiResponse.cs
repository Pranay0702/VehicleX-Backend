namespace VehicleX.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public IReadOnlyDictionary<string, string[]>? Errors { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Fail(string message, IReadOnlyDictionary<string, string[]>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = errors
        };
    }
    public static ApiResponse<T> SuccessResponse(T data, string message = "Success") => Ok(data, message);

    public static ApiResponse<T> SuccessResponse(string message, T data) => Ok(data, message);

    public static ApiResponse<T> Failure(string message, IEnumerable<string>? errors = null) => Fail(message);

    public static ApiResponse<T> FailureResponse(string message, List<string>? errors = null) =>
        Fail(message, errors?.ToDictionary(_ => "errors", e => new[] { e }));
 
}