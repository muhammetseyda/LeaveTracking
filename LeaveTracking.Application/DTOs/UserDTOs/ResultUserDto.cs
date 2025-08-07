using LeaveTracking.Application.DTOs.LeaveRequestDTOs;
using LeaveTracking.Domain.Common;
using LeaveTracking.Domain.Enums;

namespace LeaveTracking.Application.DTOs.UserDTOs
{
    public class ResultUserDto : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public int AnnualLeaveQuota { get; set; }

        public List<ResultLeaveRequestDto> LeaveRequests { get; set; }
    }
}