using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Umbral.Domain.Repositories;

namespace Umbral.Infrastructure.Middlewares;

public class ActiveUserMiddleware
{
    private readonly RequestDelegate _next;

    public ActiveUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            var tokenStamp = context.User.FindFirst("security_stamp")?.Value;

            // Only validate against DB when security_stamp claim is present.
            // This ensures backward compatibility with old tokens and test auth handlers.
            if (tokenStamp != null && userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                var user = await userRepository.GetByIdWithTrackingAsync(userId);

                if (user == null || !user.IsActive() || user.SecurityStamp != tokenStamp)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"message\":\"User account is inactive or token has been revoked.\"}");
                    return;
                }
            }
        }

        await _next(context);
    }
}
