namespace AosAdjutant.Api.Shared;

public class Result
{
    public bool IsSuccess { get; }
    protected AppError? Error { get; }

    protected Result(bool isSuccess, AppError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(AppError error) => new(false, error);

    public AppError GetError => IsSuccess ? throw new InvalidOperationException() : Error!;

    public TOut Match<TOut>(Func<TOut> onSuccess, Func<AppError, TOut> onFailure)
    {
        return IsSuccess ? onSuccess() : onFailure(Error!);
    }
}

public class Result<T> : Result
{
    protected T? Value { get; }

    public T GetValue => IsSuccess ? Value! : throw new InvalidOperationException();

    private Result(T value) : base(true, null)
    {
        Value = value;
    }

    private Result(AppError error) : base(false, error)
    {
    }

    public static Result<T> Success(T value) => new(value);
    public new static Result<T> Failure(AppError error) => new(error);

    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<AppError, TOut> onFailure)
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(Error!);
    }
}
