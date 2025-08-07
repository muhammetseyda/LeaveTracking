using AutoMapper;
using LeaveTracking.Application.DTOs.LeaveRequestDTOs;
using LeaveTracking.Application.DTOs.UserDTOs;
using LeaveTracking.Application.Helpers;
using LeaveTracking.Application.Interfaces.Repositories;
using LeaveTracking.Application.Interfaces.Services;
using LeaveTracking.Application.Shared;
using LeaveTracking.Domain.Enums;

namespace LeaveTracking.Application.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IEmailService _emailService;

        public UserServices(IUserRepository userRepository, IMapper mapper, ILeaveRequestRepository leaveRequestRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _leaveRequestRepository = leaveRequestRepository;
            _emailService = emailService;
        }

        public async Task<ResponseResult<bool>> AnnualLeaveQuotaNotificatin()
        {
            try
            {
                var users = await _userRepository.GetAnnualLeaveQuotaLessThan(5);
                if (users == null || !users.Any())
                    return ResponseResult<bool>.Failure("NOT_FOUND", "Kullanıcı bulunamadı.");
                if (users != null)
                {
                    foreach (var user in users)
                    {
                        var summary = await GetUserLeaveSummary(user.Email);
                        await _emailService.SendEmailAsync(0, 3, summary.Data);
                    }
                    return ResponseResult<bool>.SuccessResult(true);
                }
                else
                {
                    return ResponseResult<bool>.Failure("NOT_FOUND", "Kotası kritik kullanıcı bulunamadı.");
                }
            }
            catch (Exception)
            {
                return ResponseResult<bool>.Failure("EXCEPTION", "Bir hata oluştu.");
            }
        }

        public async Task<ResponseResult<bool>> Delete(string email)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user == null)
                    return ResponseResult<bool>.Failure("NOT_FOUND", "Kullanıcı bulunamadı.");
                await _userRepository.DeleteAsync(user);
                return ResponseResult<bool>.SuccessResult(true);
            }
            catch (Exception)
            {
                return ResponseResult<bool>.Failure("EXCEPTION", "Bir hata oluştu.");
            }
        }

        public async Task<ResponseResult<List<ResultUserDto>>> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                if (users == null || !users.Any())
                    return ResponseResult<List<ResultUserDto>>.Failure("NOT_FOUND", "Kullanıcı bulunamadı.");
                var result = _mapper.Map<List<ResultUserDto>>(users);
                foreach (var item in result)
                {
                    foreach (var leaveRequest in item.LeaveRequests)
                    {
                        if (leaveRequest.Status == LeaveStatus.Pending)
                        {
                            var hasconflictuser = await _leaveRequestRepository.HasConflictByUserAsync(leaveRequest.StartDate, leaveRequest.EndDate);
                            if (hasconflictuser)
                                leaveRequest.ConflictLeaveUser = true;
                        }
                    }
                    item.LeaveRequests = item.LeaveRequests.OrderByDescending(x => x.CreatedAt).ToList();
                }
                return ResponseResult<List<ResultUserDto>>.SuccessResult(result);
            }
            catch (Exception)
            {
                return ResponseResult<List<ResultUserDto>>.Failure("EXCEPTION", "Bir hata oluştu.");
            }
        }

        public async Task<ResponseResult<UserLeaveSummaryDto>> GetUserLeaveSummary(string email)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user == null)
                    return ResponseResult<UserLeaveSummaryDto>.Failure("NOT_FOUND", "Kullanıcı bulunamadı.");
                var leaveRequests = await _leaveRequestRepository.GetByUserIdAsync(user.Id);
                if (leaveRequests == null || !leaveRequests.Any())
                    return ResponseResult<UserLeaveSummaryDto>.Failure("NOT_FOUND", "Kullanıcı izin talebi bulunamadı.");
                var result = _mapper.Map<UserLeaveSummaryDto>(user);
                result.LeaveRequests = _mapper.Map<List<ResultLeaveRequestDto>>(leaveRequests);
                result.AnnualLeaveUsed = Convert.ToInt32(leaveRequests.Where(x => x.LeaveType == LeaveType.Annual && x.Status == LeaveStatus.Approved).Sum(x => LeaveCalculator.CalculateLeaveDate(x.StartDate, x.EndDate)));
                result.SickLeaveUsed = Convert.ToInt32(leaveRequests.Where(x => x.LeaveType == LeaveType.Sick && x.Status == LeaveStatus.Approved).Sum(x => LeaveCalculator.CalculateLeaveDate(x.StartDate, x.EndDate)));

                result.OtherLeaveUsed = Convert.ToInt32(leaveRequests.Where(x => (x.LeaveType == LeaveType.Bereavement || x.LeaveType == LeaveType.Maternity || x.LeaveType == LeaveType.Marriage) && x.Status == LeaveStatus.Approved).Sum(x => LeaveCalculator.CalculateLeaveDate(x.StartDate, x.EndDate)));

                result.UsedDays = result.AnnualLeaveUsed;
                result.UserPercentange = Math.Round((double)result.UsedDays / result.TotalQuota * 100, 2);
                result.LastLeaveDate = leaveRequests.Max(x => x.EndDate);
                result.RemainingDays = result.TotalQuota - result.UsedDays;

                return ResponseResult<UserLeaveSummaryDto>.SuccessResult(result);
            }
            catch (Exception)
            {
                return ResponseResult<UserLeaveSummaryDto>.Failure("EXCEPTION", "Bir hata oluştu.");
            }
        }
    }
}