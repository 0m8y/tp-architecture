namespace GestionHotel.Application.Common;

public class Result
{
    public bool IsSuccess { get; protected set; }
    public string? ErrorMessage { get; protected set; }
    public string? Message { get; protected set; }

    public static Result Success() => new Result { IsSuccess = true };

    public static Result Success(string message) => new Result { IsSuccess = true, Message = message };

    public static Result Failure(string errorMessage) => new Result { IsSuccess = false, ErrorMessage = errorMessage };
}

public class Result<T> : Result
{
    public T? Value { get; private set; }

    private Result(bool isSuccess, T? value, string? message, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Value = value;
        Message = message;
        ErrorMessage = errorMessage;
    }

    public static Result<T> Success(T value, string? message = null)
        => new(true, value, message, null);

    public static new Result<T> Failure(string errorMessage)
        => new(false, default, null, errorMessage);
}
