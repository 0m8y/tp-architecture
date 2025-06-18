namespace GestionHotel.Application.Common;

public class Result
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? Message { get; private set; }

    public static Result Success() => new Result { IsSuccess = true };

    public static Result Success(string message) => new Result { IsSuccess = true, Message = message };

    public static Result Failure(string errorMessage) => new Result { IsSuccess = false, ErrorMessage = errorMessage };
}
