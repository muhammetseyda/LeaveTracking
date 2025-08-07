using LeaveTracking.Application.DTOs.UserDTOs;
using LeaveTracking.Application.Interfaces.Services;

namespace LeaveTracking.MinimalAPI
{
    public static class AuthModule
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            //login
            app.MapPost("api/login", async (LoginDto dto, IAuthService _service) =>
            {
                var result = await _service.Login(dto);
                if (result.Success)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            }).RequireRateLimiting("sakin");
        }
    }
}