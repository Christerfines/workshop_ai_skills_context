namespace NordicBike.Portal.Middleware;

public sealed class CorrelationMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlation = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString("N");
        context.Items["correlation"] = correlation;
        context.Response.Headers["X-Correlation-ID"] = correlation;
        await _next(context);
    }
}
