using Microsoft.AspNetCore.Mvc;

namespace AosAdjutant.Api.Common;

public static class ControllerBaseExtensions
{
    public static ActionResult ApiProblem(this ControllerBase controllerBase, AppError error)
    {
        var statusCode = error.Code switch
        {
            ErrorCode.NotFound => 404,
            ErrorCode.ConcurrencyError => 409,
            ErrorCode.ValidationError => 400,
            ErrorCode.UniqueKeyError => 409,
            _ => 500
        };

        return controllerBase.StatusCode(
            statusCode,
            new ProblemDetails { Title = error.Code.ToString(), Detail = error.Message, Status = statusCode }
        );
    }
}
