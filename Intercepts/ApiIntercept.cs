using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ApiIntercept(RequestDelegate next)
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
            await context.Response.WriteAsJsonAsync(new UnauthorizedResult());
            return;
        }
    }
}
