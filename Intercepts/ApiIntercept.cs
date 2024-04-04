using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

public class ApiIntercept(RequestDelegate next, ILogger<ApiIntercept> logger)
{
    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Headers["AUTHORIZED_USER"] == Environment.UserName)
        {
            await next(context);
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
