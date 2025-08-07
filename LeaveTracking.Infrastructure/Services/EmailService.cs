using AutoMapper;
using LeaveTracking.Application.DTOs.EmailDTOs;
using LeaveTracking.Application.DTOs.UserDTOs;
using LeaveTracking.Application.Interfaces.Repositories;
using LeaveTracking.Application.Interfaces.Services;
using System.Net;
using System.Net.Mail;

namespace LeaveTracking.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IUserRepository _userRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IMapper _mapper;

        public EmailService(IEmailTemplateService emailTemplateService, ILeaveRequestRepository leaveRequestRepository, IMapper mapper, IUserRepository userRepository)
        {
            _emailTemplateService = emailTemplateService;
            _leaveRequestRepository = leaveRequestRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }
        private readonly string fromEmail = "kendi gmail hesabınız";
        private readonly string uygulamasifresi = "gmail uygulama şifresi";
        private MailMessage CreateEmailTask(string to, int emailType, EmailLeaveRequestDto? dto, UserLeaveSummaryDto? summary)
        {
            try
            {
                string konu = "İzin Talebi Sistemi";
                string body = string.Empty;
                if (emailType == 0)//0 izin talebi
                {
                    body = _emailTemplateService.GetCreateLeaveRequestTemplate(dto);
                }
                if (emailType == 1)//manger izin talepbildirimi
                {
                    body = _emailTemplateService.GetNewNotificationLeaveRequestTemplate(dto);
                }
                if (emailType == 2) //onaylandi
                {
                    body = _emailTemplateService.GetApprovedLeaveRequestTemplate(dto);
                }
                if (emailType == 3)//reddedildi
                {
                    body = _emailTemplateService.GetRejectedLeaveRequestTemplate(dto);
                }
                if (emailType == 4)//kullanici bilgildendirme
                {
                    body = _emailTemplateService.GetUserLeaveQuotaTemplate(summary);
                }

                string icerik = string.Join(Environment.NewLine, body);
                MailMessage message = new MailMessage(fromEmail, to, konu, icerik)
                {
                    IsBodyHtml = true
                };

                return message;
            }
            catch (Exception ex)
            {
                throw new Exception("E-posta gönderilirken bir hata oluştu: " + ex.Message);
            }
        }

        private async Task SendEmail(MailMessage mailmessage)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                smtpClient.Port = 587;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(fromEmail, uygulamasifresi);
                smtpClient.EnableSsl = true;

                try
                {
                    smtpClient.Send(mailmessage);
                }
                catch (Exception ex)
                {
                    throw new Exception("E-posta gönderilirken bir hata oluştu: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("E-posta gönderilirken bir hata oluştu: " + ex.Message);
            }
        }

        public async Task SendEmailAsync(int leaveRequestId, int emailType, UserLeaveSummaryDto? summary)
        {
            try
            {
                var dto = new EmailLeaveRequestDto();
                if (summary == null)
                {
                    var leaveRequest = await _leaveRequestRepository.GetByIdAsync(leaveRequestId);
                    dto = _mapper.Map<EmailLeaveRequestDto>(leaveRequest);
                    dto.Employee = _mapper.Map<EmailUserDto>(await _userRepository.GetUserByIdAsync(dto.UserId));
                    dto.Manager = _mapper.Map<EmailUserDto>(await _userRepository.GetManager());
                }

                if (emailType == 0)//bilgilendirme hem employee hem manager
                {
                    MailMessage mailMessageEmployee = CreateEmailTask(dto.Employee.Email, 0, dto, null);
                    await SendEmail(mailMessageEmployee);
                    MailMessage mailMessageManager = CreateEmailTask(dto.Manager.Email, 1, dto, null);
                    await SendEmail(mailMessageManager);
                }
                if (emailType == 1)// onaylandi sadece employee
                {
                    MailMessage mailMessageEmployee = CreateEmailTask(dto.Employee.Email, 2, dto, null);
                    await SendEmail(mailMessageEmployee);
                }
                if (emailType == 2)//reddedildi sadece employee
                {
                    MailMessage mailMessageEmployee = CreateEmailTask(dto.Employee.Email, 3, dto, null);
                    await SendEmail(mailMessageEmployee);
                }
                if (emailType == 3)// employee kalan kota bildirimi
                {
                    MailMessage mailMessageManager = CreateEmailTask(summary.Email, 4, null, summary);
                    await SendEmail(mailMessageManager);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("E-posta gönderilirken bir hata oluştu: " + ex.Message);
            }
        }
    }
}