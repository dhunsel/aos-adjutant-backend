using System.Diagnostics;

namespace AosAdjutant.Api.Common;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = Activity.Current?.TraceId.ToString();

        context.Response.Headers["X-Trace-ID"] = traceId;

        await next(context);
    }
}
