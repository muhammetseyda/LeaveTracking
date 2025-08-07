using LeaveTracking.Application.DTOs.UserDTOs;
using LeaveTracking.Application.Shared;

namespace LeaveTracking.Application.Interfaces.Services
{
    public interface IUserServices
    {
        Task<ResponseResult<List<ResultUserDto>>> GetAllUsers();

        Task<ResponseResult<bool>> Delete(string email);

        Task<ResponseResult<UserLeaveSummaryDto>> GetUserLeaveSummary(string email);

        Task<ResponseResult<bool>> AnnualLeaveQuotaNotificatin();//background worker calisabilir.
    }
}