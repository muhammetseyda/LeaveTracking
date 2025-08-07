using LeaveTracking.Application.DTOs.UserDTOs;
using LeaveTracking.Domain.Common;
using LeaveTracking.Domain.Enums;

namespace LeaveTracking.Application.DTOs.LeaveRequestDTOs
{
    public class LeaveRequestDto : BaseEntity
    {
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveType LeaveType { get; set; }
        public string? Description { get; set; }
        public LeaveStatus Status { get; set; }
        public string? RejectedReason { get; set; }
        public bool? ConflictLeaveUser { get; set; }

        public UserDto User { get; set; }
    }
}