using LeaveTracking.Domain.Enums;

namespace LeaveTracking.Application.DTOs.EmailDTOs
{
    public class EmailUserDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public int AnnualLeaveQuota { get; set; }
    }
}