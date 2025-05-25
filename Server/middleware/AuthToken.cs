using JWT.Builder;
using JWT.Algorithms;


namespace Server.Middleware;

public class AuthToken : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {

        string path = context.Request.Path;

        if (!path.Contains("/Api") || path.Contains("/Auth") || path.Contains("/GetStock") || path.Contains("/GetItem") || path.Contains("/Search"))
        {
            await next(context);
            return;
        }

        string secret = Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET")!;
        string? headers = context.Request.Headers.Authorization;

        if (headers == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Auth header was null");
            return;
        }

        try
        {
            string authHeader = headers.Split(" ")[1];
            var decoded = JwtBuilder
                .Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(secret)
                .Decode(authHeader);
            

            if (decoded == null)
            {
                await context.Response.WriteAsync("token invalid");
                context.Response.StatusCode = 401;
                return;
            }

            IDictionary<object, object?> item = new Dictionary<object, object?>
            {
                {"tokenData", decoded}
            };

            context.Items = item;
            await next(context);
        } catch (Exception err)
        {
            System.Console.WriteLine(err.Message);
            context.Response.StatusCode = 500;
            return;
        }
    }
}

public static class AuthTokenExtensions
{
    public static IApplicationBuilder UseAuthToken(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthToken>();
    }
}
