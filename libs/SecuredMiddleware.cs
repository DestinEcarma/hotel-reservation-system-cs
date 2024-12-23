using hotel_reservation_system.Models;
using JWT.Builder;
using System.Text.Json;

namespace hotel_reservation_system.libs
{
    public class SecuredMiddleware
    {
        private readonly RequestDelegate _next;

        public SecuredMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/admin") || context.Request.Path.StartsWithSegments("/api/staff"))
            {
                if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    try
                    {
                        var token = authHeader.ToString().Replace("Bearer ", string.Empty);

                        var sessionJson = JwtBuilder.Create()
                            .WithAlgorithm(new JWT.Algorithms.HMACSHA256Algorithm())
                            .WithSecret(DotNetEnv.Env.GetString("JWT_SECRET"))
                            .Decode(token);

                        context.Items["session"] = JsonSerializer.Deserialize<User>(sessionJson);

                        await _next(context);
                        return;
                    }
                    catch { }
                }

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            await _next(context);
        }
    }
}
