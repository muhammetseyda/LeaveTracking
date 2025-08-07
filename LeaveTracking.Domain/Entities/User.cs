using LeaveTracking.Domain.Common;
using LeaveTracking.Domain.Enums;

namespace LeaveTracking.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public int AnnualLeaveQuota { get; set; } = 14;
        public List<LeaveRequest> LeaveRequests { get; set; }
    }
}