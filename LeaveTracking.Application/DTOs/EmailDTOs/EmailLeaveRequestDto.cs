using LeaveTracking.Domain.Enums;

namespace LeaveTracking.Application.DTOs.EmailDTOs
{
    public class EmailLeaveRequestDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveType LeaveType { get; set; }
        public string? Description { get; set; }
        public LeaveStatus Status { get; set; }
        public string? RejectedReason { get; set; }
        public bool? ConflictLeaveUser { get; set; }

        public EmailUserDto Employee { get; set; }
        public EmailUserDto Manager { get; set; }
    }
}