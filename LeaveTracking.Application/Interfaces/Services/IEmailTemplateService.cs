using LeaveTracking.Application.DTOs.EmailDTOs;
using LeaveTracking.Application.DTOs.UserDTOs;

namespace LeaveTracking.Application.Interfaces.Services
{
    public interface IEmailTemplateService
    {
        string GetCreateLeaveRequestTemplate(EmailLeaveRequestDto dto);

        string GetApprovedLeaveRequestTemplate(EmailLeaveRequestDto dto);

        string GetNewNotificationLeaveRequestTemplate(EmailLeaveRequestDto dto);

        string GetRejectedLeaveRequestTemplate(EmailLeaveRequestDto dto);

        string GetUserLeaveQuotaTemplate(UserLeaveSummaryDto summary);
    }
}