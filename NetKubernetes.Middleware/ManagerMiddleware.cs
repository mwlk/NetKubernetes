using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NetKubernetes.Middleware;

public class ManagerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ManagerMiddleware> _logger;

    public ManagerMiddleware(RequestDelegate next, ILogger<ManagerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            await ManagetExceptionAsync(context, e, _logger);
        }
    }

    private async Task ManagetExceptionAsync(HttpContext context, Exception e, ILogger<ManagerMiddleware> logger)
    {
        object? errors = null;

        switch (e)
        {
            case MiddlewareException me:
                logger.LogError(e, "Middleware Error");
                errors = me.Errors;
                context.Response.StatusCode = (int)me.StatusCode;
                break;

            case Exception ex:
                logger.LogError(e, "Server Error");
                errors = string.IsNullOrWhiteSpace(e.Message) ? "Error" : e.Message;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        context.Response.ContentType = "application/json";
        var results = string.Empty;
        if (errors != null)
        {
            results = JsonConvert.SerializeObject(new { errors });
        }

        await context.Response.WriteAsync(results);
    }
}