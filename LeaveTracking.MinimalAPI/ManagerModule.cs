using LeaveTracking.Application.DTOs.LeaveRequestDTOs;
using LeaveTracking.Application.DTOs.UserDTOs;
using LeaveTracking.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeaveTracking.MinimalAPI
{
    public static class ManagerModule
    {
        public static void MapManagerEndpoints(this IEndpointRouteBuilder app)
        {
            //register
            app.MapPost("/api/user", async (RegisterDto dto, IAuthService _service) =>
            {
                var result = await _service.Register(dto);
                if (result.Success)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            }).RequireAuthorization("RequireManager").RequireRateLimiting("sakin");

            //leave request approve
            app.MapPost("api/leaverequest/approve", async ([FromQuery] int id, ILeaveRequestService _services) =>
            {
                var result = await _services.ApproveLeaveRequest(id);
                if (result.Success)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            }).RequireAuthorization("RequireManager").RequireRateLimiting("sakin");
            //leave request reject
            app.MapPost("api/leaverequest/reject", async (RejectLeaveRequestDto dto, ILeaveRequestService _services) =>
            {
                var result = await _services.RejectLeaveRequest(dto.LeaveRequestId, dto.RejectReason);
                if (result.Success)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            }).RequireAuthorization("RequireManager").RequireRateLimiting("sakin");
            //leave request update
            app.MapPut("api/leaverequest", async (UpdateLeaveRequestDto dto, ILeaveRequestService _services) =>
            {
                var result = await _services.UpdateLeaveRequest(dto);
                if (result.Success)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            }).RequireAuthorization().RequireRateLimiting("sakin");
            //leave request get all
            app.MapGet("api/leaverequest", async (ILeaveRequestService _services) =>
            {
                var result = await _services.GetAllLeaveRequests();
                if (result.Success)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            }).RequireAuthorization("RequireManager").RequireRateLimiting("sakin");

            //leave request pendign
            app.MapGet("api/leaverequest/pending", async (ILeaveRequestService _services) =>
            {
                var result = await _services.GetPendingLeaveRequests();
                if (result.Success)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            }).RequireAuthorization("RequireManager").RequireRateLimiting("sakin");
            //get manager dashboard
            app.MapGet("api/manager/dash", async (ILeaveRequestService _services) =>
            {
                var result = await _services.GetManagerDash();
                if (result.Success)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            }).RequireAuthorization("RequireManager").RequireRateLimiting("sakin");
            //get all users include lleaverequests
            app.MapGet("api/users", async (IUserServices _services) =>
            {
                var result = await _services.GetAllUsers();
                if (result.Success)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            }).RequireAuthorization("RequireManager").RequireRateLimiting("sakin");
            app.MapDelete("api/user", async ([FromQuery] string email, IUserServices _services) =>
            {
                var result = await _services.Delete(email);
                if (result.Success)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            }).RequireAuthorization("RequireManager").RequireRateLimiting("sakin");

            app.MapPost("api/user/annualleavequota", async (IUserServices _services) =>
            {
                var result = await _services.AnnualLeaveQuotaNotificatin();
                if (result.Success)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            }).RequireAuthorization("RequireManager").RequireRateLimiting("sakin");
        }
    }
}