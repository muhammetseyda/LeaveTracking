using LeaveTracking.Domain.Enums;

namespace LeaveTracking.Application.DTOs.UserDTOs
{
    public class UserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public int AnnualLeaveQuota { get; set; }
    }
}