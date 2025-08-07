using LeaveTracking.Application.DTOs.LeaveRequestDTOs;
using LeaveTracking.Domain.Common;
using LeaveTracking.Domain.Enums;

namespace LeaveTracking.Application.DTOs.UserDTOs
{
    public class UserLeaveSummaryDto : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public int AnnualLeaveQuota { get; set; }
        public int TotalQuota { get; set; } = 14;
        public int UsedDays { get; set; } = 0;
        public int RemainingDays { get; set; } = 0;
        public double UserPercentange { get; set; } = 0.0;
        public int AnnualLeaveUsed { get; set; } = 0;
        public int SickLeaveUsed { get; set; } = 0;
        public int OtherLeaveUsed { get; set; } = 14;
        public DateTime LastLeaveDate { get; set; }

        public List<ResultLeaveRequestDto> LeaveRequests { get; set; }
    }
}