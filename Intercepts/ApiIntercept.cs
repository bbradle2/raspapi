using Microsoft.AspNetCore.Mvc;

namespace raspapi.Intercepts
{
    public class ApiIntercept(RequestDelegate next, ILogger<ApiIntercept> logger)
    {

        public async Task InvokeAsync(HttpContext context)
        {

            context.Response.Headers.Remove("GitSemVer");
            context.Response.Headers.Append("GitSemVer", GitVersionInformation.SemVer);
            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();

            if (context.Request.Headers["AUTHORIZED_USER"] == Environment.UserName)
            {
                logger.LogInformation("Authorized User {Environment.UserName}", Environment.UserName);
                await next(context);
                return;
            }
 
            context.Response.StatusCode = 401;
            string unauthorizedUser = context.Request.Headers["AUTHORIZED_USER"]!;
            logger.LogError("Unauthorized User {unauthorizedUser}", unauthorizedUser);
            await context.Response.WriteAsJsonAsync(new UnauthorizedResult());
            return;
 
        }
    }
}
