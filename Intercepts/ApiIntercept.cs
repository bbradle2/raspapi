namespace raspapi.Intercepts
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    public class ApiIntercept(RequestDelegate next, ILogger<ApiIntercept> logger)
    {

        public async Task InvokeAsync(HttpContext context)
        {

            context.Response.Headers.Remove("GitSemVer");
            context.Response.Headers.Append("GitSemVer", GitVersionInformation.SemVer);
            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();

            if (context.Request.Headers["AUTHORIZED_USER"] == Environment.UserName)
            {
                await next(context);
            }
            else
            {
                context.Response.StatusCode = 401;
                string unauthorizedUser = context.Request.Headers["AUTHORIZED_USER"]!;
                logger.LogError("UnauthorizedUser {unauthorizedUser}", unauthorizedUser);
                await context.Response.WriteAsJsonAsync(new UnauthorizedResult());
                return;
            }
        }
    }
}
