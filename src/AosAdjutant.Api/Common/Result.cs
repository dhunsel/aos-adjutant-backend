#pragma warning disable CA1000 // Static members on generic types are intentional here for factory method ergonomics
namespace AosAdjutant.Api.Common;

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

public sealed class Result<T> : Result
{
    private T? Value { get; }

    public T GetValue => IsSuccess ? Value! : throw new InvalidOperationException();

    private Result(T value) : base(true, null)
    {
        Value = value;
    }

    private Result(AppError error) : base(false, error)
    {
    }

    public static Result<T> Success(T value) => new(value);
    public static new Result<T> Failure(AppError error) => new(error);

    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<AppError, TOut> onFailure)
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(Error!);
    }
}
