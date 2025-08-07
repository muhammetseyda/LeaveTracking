using LeaveTracking.Domain.Common;
using LeaveTracking.Domain.Enums;

namespace LeaveTracking.Domain.Entities
{
    public class LeaveRequest : BaseEntity
    {
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveType LeaveType { get; set; }
        public string? Description { get; set; }
        public LeaveStatus Status { get; set; }
        public string? RejectedReason { get; set; }

        public User User { get; set; }
    }
}