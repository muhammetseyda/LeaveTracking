using AutoMapper;
using FluentValidation;
using LeaveTracking.Application.DTOs.LeaveRequestDTOs;
using LeaveTracking.Application.DTOs.WebDTOs;
using LeaveTracking.Application.Helpers;
using LeaveTracking.Application.Interfaces.Repositories;
using LeaveTracking.Application.Interfaces.Services;
using LeaveTracking.Application.Shared;
using LeaveTracking.Domain.Entities;
using LeaveTracking.Domain.Enums;

namespace LeaveTracking.Application.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IValidator<CreateLeaveRequestDto> _createValidator;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _emailService;

        public LeaveRequestService(ILeaveRequestRepository leaveRequestRepository, IValidator<CreateLeaveRequestDto> createValidator, IMapper mapper, IUserRepository userRepository, ICurrentUserService currentUserService, IEmailService emailService)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _createValidator = createValidator;
            _mapper = mapper;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _emailService = emailService;
        }

        public async Task<ResponseResult<string>> ApproveLeaveRequest(int requestId)
        {
            try
            {
                var item = await _leaveRequestRepository.GetByIdAsync(requestId);
                if (item == null)
                    return ResponseResult<string>.Failure("NOT_FOUND", "İstek bulunamadı.");

                item.Status = LeaveStatus.Approved;

                var leaveDate = LeaveCalculator.CalculateLeaveDate(item.StartDate, item.EndDate);
                if (item.LeaveType == LeaveType.Annual)
                    await _userRepository.UpdateAnnualLeaveQuotaAsync(item.UserId, leaveDate);
                item.UpdatedBy = _currentUserService.UserId;
                await _leaveRequestRepository.UpdateAsync(item);
                await _emailService.SendEmailAsync(item.Id, 1, null);
                return ResponseResult<string>.SuccessResult("TRUE");
            }
            catch (Exception)
            {
                return ResponseResult<string>.Failure("EXCEPTION", "Bir hata oluştu.");
            }
        }

        public async Task<ResponseResult<string>> CreateLeaveRequest(CreateLeaveRequestDto dto)
        {
            try
            {
                var validate = await _createValidator.ValidateAsync(dto);
                if (!validate.IsValid)
                    return ResponseResult<string>.Failure("VALIDATION_ERROR", validate.Errors.Select(x => x.ErrorMessage).FirstOrDefault());

                var checkuser = await _userRepository.CheckUserAsync(dto.UserId);
                if (!checkuser)
                    return ResponseResult<string>.Failure("NOT_FOUND", "Kullanıcı bulunamadı.");
                var checkdate = await _leaveRequestRepository.HasConflictAsync(dto.UserId, dto.StartDate, dto.EndDate);
                if (checkdate)
                    return ResponseResult<string>.Failure("CONFLICT", "Bu tarihlerde zaten bir izin isteği var");
                var userQuoata = await _userRepository.GetUserAnnualLeaveQuota(dto.UserId);
                if (userQuoata == 0)
                    return ResponseResult<string>.Failure("FULL_QUOTA", "Kullanıcının izin hakkı bitmiştir.");
                var leaveDate = LeaveCalculator.CalculateLeaveDate(dto.StartDate, dto.EndDate);
                if (userQuoata < leaveDate)
                    return ResponseResult<string>.Failure("FAIL_QUOTA", "Kullanıcının izin hakkı yetersizdir.");
                var item = _mapper.Map<LeaveRequest>(dto);
                item.CreatedBy = _currentUserService.UserId;

                await _leaveRequestRepository.CreateAsync(item);

                await _emailService.SendEmailAsync(item.Id, 0, null);
                return ResponseResult<string>.SuccessResult("TRUE");
            }
            catch (Exception)
            {
                return ResponseResult<string>.Failure("EXCEPTION", "Bir hata oluştu");
            }
        }

        public async Task<ResponseResult<bool>> DeleteLeaveRequest(int id)
        {
            try
            {
                var item = await _leaveRequestRepository.GetByIdAsync(id);
                if (item == null)
                    return ResponseResult<bool>.Failure("NOT_FOUND", "İstek bulunamadı.");
                await _leaveRequestRepository.DeleteAsync(item);
                return ResponseResult<bool>.SuccessResult(true);
            }
            catch (Exception)
            {
                return ResponseResult<bool>.Failure("EXCEPTION", "Bir hata oluştu");
            }
        }

        public async Task<ResponseResult<List<LeaveRequestDto>>> GetAllLeaveRequests()
        {
            try
            {
                var item = await _leaveRequestRepository.GetAllAsync();
                var result = _mapper.Map<List<LeaveRequestDto>>(item);

                foreach (var request in result)
                {
                    if (request.Status == LeaveStatus.Pending)
                    {
                        var hasconflictuser = await _leaveRequestRepository.HasConflictByUserAsync(request.StartDate, request.EndDate);
                        if (hasconflictuser)
                            request.ConflictLeaveUser = true;
                    }
                }
                return ResponseResult<List<LeaveRequestDto>>.SuccessResult(result);
            }
            catch (Exception)
            {
                return ResponseResult<List<LeaveRequestDto>>.Failure("EXCEPTION", "Bir hata oluştu.");
            }
        }

        public async Task<ResponseResult<EmployeeDash>> GetEmployeeDash()
        {
            try
            {
                var dash = new EmployeeDash();
                var leaverequests = await _leaveRequestRepository.GetByUserIdAsync(Convert.ToInt32(_currentUserService.UserId));
                dash.PendingLeave = leaverequests.Count(x => x.Status == LeaveStatus.Pending);
                dash.RemainingLeave = await _userRepository.GetUserAnnualLeaveQuota(Convert.ToInt32(_currentUserService.UserId));
                dash.MounthLeave = leaverequests.Count(x => x.CreatedAt.Month == DateTime.Now.Month && x.CreatedAt.Year == DateTime.Now.Year && x.Status == LeaveStatus.Approved);
                dash.TotalLeave = 14;
                return ResponseResult<EmployeeDash>.SuccessResult(dash);
            }
            catch (Exception)
            {
                return ResponseResult<EmployeeDash>.Failure("EXCEPTION", "Bir hata oluştu.");
            }
        }

        public async Task<ResponseResult<LeaveRequestDto>> GetLeaveRequestById(int requestId)
        {
            try
            {
                var item = await _leaveRequestRepository.GetByIdAsync(requestId);
                if (item == null)
                    return ResponseResult<LeaveRequestDto>.Failure("NOT_FOUND", "İstek bulunamadı.");
                var result = _mapper.Map<LeaveRequestDto>(item);
                return ResponseResult<LeaveRequestDto>.SuccessResult(result);
            }
            catch (Exception)
            {
                return ResponseResult<LeaveRequestDto>.Failure("EXCEPTION", "Bir hata oluştu.");
            }
        }

        public async Task<ResponseResult<ManagerDash>> GetManagerDash()
        {
            try
            {
                var dash = new ManagerDash();
                var leaverequests = await _leaveRequestRepository.GetAllAsync();
                dash.PendingRequest = leaverequests.Count(x => x.Status == LeaveStatus.Pending);
                dash.ApproveRequest = leaverequests.Count(x => x.Status == LeaveStatus.Approved);
                dash.RejectRequest = leaverequests.Count(x => x.Status == LeaveStatus.Rejected);
                dash.MounthRequest = leaverequests.Count(x => x.CreatedAt.Month == DateTime.Now.Month && x.CreatedAt.Year == DateTime.Now.Year);
                return ResponseResult<ManagerDash>.SuccessResult(dash);
            }
            catch (Exception)
            {
                return ResponseResult<ManagerDash>.Failure("EXCEPTION", "Bir hata oluştu.");
            }
        }

        public async Task<ResponseResult<List<LeaveRequestDto>>> GetPendingLeaveRequests()
        {
            try
            {
                var item = await _leaveRequestRepository.GetPendingAsync();
                var result = _mapper.Map<List<LeaveRequestDto>>(item);
                return ResponseResult<List<LeaveRequestDto>>.SuccessResult(result);
            }
            catch (Exception)
            {
                return ResponseResult<List<LeaveRequestDto>>.Failure("EXCEPTION", "Bir hata oluştu.");
            }
        }

        public async Task<ResponseResult<List<LeaveRequestDto>>> GetUserLeaveRequests(int userId)
        {
            try
            {
                var item = await _leaveRequestRepository.GetByUserIdAsync(userId);
                var result = _mapper.Map<List<LeaveRequestDto>>(item);
                return ResponseResult<List<LeaveRequestDto>>.SuccessResult(result);
            }
            catch (Exception)
            {
                return ResponseResult<List<LeaveRequestDto>>.Failure("EXCEPTION", "Bir hata oluştu.");
            }
        }

        public async Task<ResponseResult<string>> RejectLeaveRequest(int requestId, string? rejectedReason)
        {
            try
            {
                var item = await _leaveRequestRepository.GetByIdAsync(requestId);
                if (item == null)
                    return ResponseResult<string>.Failure("NOT_FOUND", "İstek bulunamadı.");

                item.Status = LeaveStatus.Rejected;
                item.RejectedReason = rejectedReason;
                item.UpdatedBy = _currentUserService.UserId;
                await _leaveRequestRepository.UpdateAsync(item);
                await _emailService.SendEmailAsync(item.Id, 2, null);
                return ResponseResult<string>.SuccessResult("TRUE");
            }
            catch (Exception)
            {
                return ResponseResult<string>.Failure("EXCEPTION", "Bir hata oluştu.");
            }
        }

        public async Task<ResponseResult<string>> UpdateLeaveRequest(UpdateLeaveRequestDto dto)
        {
            try
            {
                var item = await _leaveRequestRepository.GetByIdAsync(dto.Id);
                if (item == null)
                    return ResponseResult<string>.Failure("NOT_FOUND", "İstek bulunamadı.");
                var checkdate = await _leaveRequestRepository.HasConflictAsync(dto.UserId, dto.StartDate, dto.EndDate);
                if (!checkdate)
                    return ResponseResult<string>.Failure("CONFLICT", "Bu tarihlerde zaten bir izin isteği var.");

                var result = _mapper.Map(dto, item);
                result.UpdatedBy = _currentUserService.UserId;
                await _leaveRequestRepository.UpdateAsync(result);

                return ResponseResult<string>.SuccessResult("TRUE");
            }
            catch (Exception)
            {
                return ResponseResult<string>.Failure("EXCEPTION", "Bir hata oluştu.");
            }
        }
    }
}