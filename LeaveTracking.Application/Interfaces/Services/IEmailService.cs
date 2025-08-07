using LeaveTracking.Application.DTOs.UserDTOs;

namespace LeaveTracking.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(int leaveRequestId, int emailType, UserLeaveSummaryDto? summary);
    }
}