using LeaveTracking.Application.DTOs.LeaveRequestDTOs;
using LeaveTracking.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeaveTracking.MinimalAPI
{
    public static class UserModule
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            //create leave request
            app.MapPost("api/leaverequest", async (CreateLeaveRequestDto dto, ILeaveRequestService _service) =>
            {
                var result = await _service.CreateLeaveRequest(dto);
                if (result.Success)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            }).RequireAuthorization().RequireRateLimiting("sakin");
            //user leave request
            app.MapGet("api/leaverequest/user", async ([FromQuery] int userId, ILeaveRequestService _services) =>
            {
                var result = await _services.GetUserLeaveRequests(userId);
                if (result.Success)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            }).RequireAuthorization().RequireRateLimiting("sakin");
            app.MapGet("api/employee/dash", async (ILeaveRequestService _services) =>
            {
                var result = await _services.GetEmployeeDash();
                if (result.Success)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            }).RequireAuthorization().RequireRateLimiting("sakin");
            //delete leave request
            app.MapDelete("api/leaverequest", async ([FromQuery] int id, ILeaveRequestService _services) =>
            {
                var result = await _services.DeleteLeaveRequest(id);
                if (result.Success)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            }).RequireAuthorization().RequireRateLimiting("sakin");
        }
    }
}