#pragma warning disable MA0048
namespace AosAdjutant.Api.Common;

public enum ErrorCode
{
    NotFound,
    ValidationError,
    ConcurrencyError,
    UniqueKeyError,
}

public sealed class AppError(ErrorCode code, string message)
{
    public ErrorCode Code { get; } = code;
    public string Message { get; } = message;
}
