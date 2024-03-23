using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ApiIntercept
{
   
    private readonly RequestDelegate _next;
    public ApiIntercept(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Headers["AUTHORIZED_USER"] != Environment.UserName)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new UnauthorizedResult());
            return;
        } 

        await _next(context);
    }
}
