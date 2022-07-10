namespace TP3VeilleSignalR.Utilities;

public class Result
{
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }

    protected Result(bool isSuccess, string? error)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Cannot have an error in case of a successful result");
        if (!isSuccess && string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Cannot have a failed result with an empty error");
        
        IsSuccess = isSuccess;
        Error = error;
    }
    public static Result Ok() => new Result(true, string.Empty);
    public static Result Fail(string error) => new (false, error);
    public static Result<T> Ok<T>(T value) => new(value, string.Empty, true);
    public static Result<T> Fail<T>(string error) => new(default(T), error, false);
  
}

public class Result<T> : Result
{
    public T? Value { get; init; }
    protected internal Result(T? value, string error, bool isSuccess) : base(isSuccess, error)
    {
        Value = value;
    }
}