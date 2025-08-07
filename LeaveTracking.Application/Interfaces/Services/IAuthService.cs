using LeaveTracking.Application.DTOs.UserDTOs;
using LeaveTracking.Application.Shared;

namespace LeaveTracking.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ResponseResult<string>> Login(LoginDto dto);

        Task<ResponseResult<string>> Register(RegisterDto dto);
    }
}