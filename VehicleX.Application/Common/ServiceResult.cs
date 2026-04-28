namespace VehicleX.Application.Common;

public class ServiceResult<T>
{
    private ServiceResult(bool isSuccess, string message, T? data, ResultErrorType errorType)
    {
        IsSuccess = isSuccess;
        Message = message;
        Data = data;
        ErrorType = errorType;
    }

    public bool IsSuccess { get; }

    public string Message { get; }

    public T? Data { get; }

    public ResultErrorType ErrorType { get; }

    public static ServiceResult<T> Success(T data, string message)
    {
        return new ServiceResult<T>(true, message, data, ResultErrorType.None);
    }

    public static ServiceResult<T> ValidationFailure(string message)
    {
        return new ServiceResult<T>(false, message, default, ResultErrorType.Validation);
    }

    public static ServiceResult<T> NotFound(string message)
    {
        return new ServiceResult<T>(false, message, default, ResultErrorType.NotFound);
    }

    public static ServiceResult<T> Conflict(string message)
    {
        return new ServiceResult<T>(false, message, default, ResultErrorType.Conflict);
    }

    public static ServiceResult<T> Failure(string message)
    {
        return new ServiceResult<T>(false, message, default, ResultErrorType.Error);
    }
}
