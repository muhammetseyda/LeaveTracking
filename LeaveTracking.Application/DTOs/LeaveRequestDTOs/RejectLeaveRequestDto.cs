namespace LeaveTracking.Application.DTOs.LeaveRequestDTOs
{
    public class RejectLeaveRequestDto
    {
        public int LeaveRequestId { get; set; }
        public string? RejectReason { get; set; }
    }
}