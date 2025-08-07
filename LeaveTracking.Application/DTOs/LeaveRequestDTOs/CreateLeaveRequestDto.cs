using LeaveTracking.Domain.Enums;

namespace LeaveTracking.Application.DTOs.LeaveRequestDTOs
{
    public class CreateLeaveRequestDto
    {
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveType LeaveType { get; set; }
        public string? Description { get; set; }
    }
}