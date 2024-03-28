using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

public class ApiIntercept(RequestDelegate next, ILogger<ApiIntercept> logger)
{
    private readonly RequestDelegate _next = next;
    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Headers["AUTHORIZED_USER"] == Environment.UserName)
        {
            await _next(context);
        }
        else
        {
            context.Response.StatusCode = 401;
            logger.LogError($"Invalid User {context.Request.Headers["AUTHORIZED_USER"]}");
            await context.Response.WriteAsJsonAsync(new UnauthorizedResult());
            return;
        }
    }
}
