using LeaveTracking.Application.DTOs.LeaveRequestDTOs;
using LeaveTracking.Application.DTOs.WebDTOs;
using LeaveTracking.Application.Shared;

namespace LeaveTracking.Application.Interfaces.Services
{
    public interface ILeaveRequestService
    {
        Task<ResponseResult<string>> CreateLeaveRequest(CreateLeaveRequestDto dto);

        Task<ResponseResult<string>> UpdateLeaveRequest(UpdateLeaveRequestDto dto);

        Task<ResponseResult<string>> ApproveLeaveRequest(int requestId);

        Task<ResponseResult<string>> RejectLeaveRequest(int requestId, string? rejectedReason);

        Task<ResponseResult<List<LeaveRequestDto>>> GetAllLeaveRequests();

        Task<ResponseResult<List<LeaveRequestDto>>> GetUserLeaveRequests(int userId);

        Task<ResponseResult<LeaveRequestDto>> GetLeaveRequestById(int requestId);

        Task<ResponseResult<List<LeaveRequestDto>>> GetPendingLeaveRequests();

        Task<ResponseResult<ManagerDash>> GetManagerDash();

        Task<ResponseResult<EmployeeDash>> GetEmployeeDash();

        Task<ResponseResult<bool>> DeleteLeaveRequest(int id);
    }
}