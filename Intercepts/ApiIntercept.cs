using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

public class ApiIntercept(RequestDelegate next, ILogger<ApiIntercept> logger)
{
   
    public async Task InvokeAsync(HttpContext context)
    {
       
        string semVer = GitVersionInformation.SemVer;
        context.Request.Headers.Remove("GitSemVer");
        context.Request.Headers.Append("GitSemVer", semVer);

        if (context.Request.Headers["AUTHORIZED_USER"] == Environment.UserName)
        {
            await next(context);
        }
        else
        {
            context.Response.StatusCode = 401;
            
            string authorizedUser = context.Request.Headers["AUTHORIZED_USER"]!;
            logger.LogError("Invalid User {AuthorizedUser}", authorizedUser);
            await context.Response.WriteAsJsonAsync(new UnauthorizedResult());
            return;
        }
    }
}
