using LeaveTracking.Application.DTOs.EmailDTOs;
using LeaveTracking.Application.DTOs.UserDTOs;
using LeaveTracking.Application.Helpers;
using LeaveTracking.Application.Interfaces.Services;

namespace LeaveTracking.Infrastructure.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly string _templateFolder;

        public EmailTemplateService(string templateFolder)
        {
            _templateFolder = templateFolder;
        }

        public string GetApprovedLeaveRequestTemplate(EmailLeaveRequestDto dto)
        {
            var path = Path.Combine(_templateFolder, "ApprovedLeaveRequestTemplate.html");
            var template = File.ReadAllText(path);
            template = template.Replace("{FullName}", (dto.Employee.FirstName + " " + dto.Employee.LastName))
                               .Replace("{RequestId}", dto.Id.ToString())
                               .Replace("{LeaveType}", LeaveTypeHelpers.GetLeaveTypeName(dto.LeaveType))
                               .Replace("{StartDate}", dto.StartDate.ToString("dd.MM.yyyy"))
                               .Replace("{EndDate}", dto.EndDate.ToString("dd.MM.yyyy"))
                               .Replace("{TotalDays}", LeaveCalculator.CalculateLeaveDate(dto.StartDate, dto.EndDate).ToString())
                               .Replace("{ManagerName}", (dto.Manager.FirstName + " " + dto.Manager.LastName))
                               .Replace("{ApprovalDate}", dto.UpdatedAt.ToString("dd.MM.yyyy"))
                               .Replace("{RemainingDays}", dto.Employee.AnnualLeaveQuota.ToString());

            return template;
        }

        public string GetCreateLeaveRequestTemplate(EmailLeaveRequestDto dto)
        {
            var path = Path.Combine(_templateFolder, "CreateLeaveRequestTemplate.html");
            var template = File.ReadAllText(path);
            template = template.Replace("{FullName}", (dto.Employee.FirstName + " " + dto.Employee.LastName))
                               .Replace("{RequestId}", dto.Id.ToString())
                               .Replace("{LeaveType}", LeaveTypeHelpers.GetLeaveTypeName(dto.LeaveType))
                               .Replace("{StartDate}", dto.StartDate.ToString("dd.MM.yyyy"))
                               .Replace("{EndDate}", dto.EndDate.ToString("dd.MM.yyyy"))
                               .Replace("{TotalDays}", LeaveCalculator.CalculateLeaveDate(dto.StartDate, dto.EndDate).ToString());

            return template;
        }

        public string GetNewNotificationLeaveRequestTemplate(EmailLeaveRequestDto dto)
        {
            var path = Path.Combine(_templateFolder, "NewNotificationLeaveRequestTemplate.html");
            var template = File.ReadAllText(path);
            template = template.Replace("{ManagerName}", (dto.Manager.FirstName + " " + dto.Manager.LastName))
                               .Replace("{EmployeeName}", (dto.Employee.FirstName + " " + dto.Employee.LastName))
                               .Replace("{RequestId}", dto.Id.ToString())
                               .Replace("{LeaveType}", LeaveTypeHelpers.GetLeaveTypeName(dto.LeaveType))
                               .Replace("{StartDate}", dto.StartDate.ToString("dd.MM.yyyy"))
                               .Replace("{EndDate}", dto.EndDate.ToString("dd.MM.yyyy"))
                               .Replace("{TotalDays}", LeaveCalculator.CalculateLeaveDate(dto.StartDate, dto.EndDate).ToString())
                               .Replace("{RequestDate}", dto.CreatedAt.ToString("dd.MM.yyyy"))
                               .Replace("{EmployeeRemainingDays}", dto.Employee.AnnualLeaveQuota.ToString());

            return template;
        }

        public string GetRejectedLeaveRequestTemplate(EmailLeaveRequestDto dto)
        {
            var path = Path.Combine(_templateFolder, "RejectedLeaveRequestTemplate.html");
            var template = File.ReadAllText(path);
            template = template.Replace("{FullName}", (dto.Employee.FirstName + " " + dto.Employee.LastName))
                              .Replace("{LeaveType}", LeaveTypeHelpers.GetLeaveTypeName(dto.LeaveType))
                              .Replace("{StartDate}", dto.StartDate.ToString("dd.MM.yyyy"))
                              .Replace("{EndDate}", dto.EndDate.ToString("dd.MM.yyyy"))
                              .Replace("{TotalDays}", LeaveCalculator.CalculateLeaveDate(dto.StartDate, dto.EndDate).ToString())
                              .Replace("{RejectionReason}", dto.RejectedReason == null ? " - " : dto.RejectedReason.ToString());

            return template;
        }

        public string GetUserLeaveQuotaTemplate(UserLeaveSummaryDto dto)
        {
            var path = Path.Combine(_templateFolder, "UserLeaveQuotaTemplate.html");
            //var data = await _userServices.GetUserLeaveSummary(email);
            //var dto = data.Data;
            var template = File.ReadAllText(path);
            template = template.Replace("{FullName}", (dto.FirstName + " " + dto.LastName))
                              .Replace("{TotalQuota}", dto.TotalQuota.ToString())
                              .Replace("{UsedDays}", dto.UsedDays.ToString())
                              .Replace("{RemainingDays}", dto.RemainingDays.ToString())
                              .Replace("{UsagePercentage}", dto.UserPercentange.ToString())
                              .Replace("{AnnualLeaveUsed}", dto.AnnualLeaveUsed.ToString())
                              .Replace("{AnnualLeaveTotal}", dto.TotalQuota.ToString())
                              .Replace("{SickLeaveUsed}", dto.SickLeaveUsed.ToString())
                              .Replace("{OtherLeaveUsed}", dto.OtherLeaveUsed.ToString())
                              .Replace("{LastLeaveDate}", dto.LastLeaveDate.ToString("dd.MM.yyyy"));
            return template;
        }
    }
}