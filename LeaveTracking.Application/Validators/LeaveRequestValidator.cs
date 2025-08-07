using FluentValidation;
using LeaveTracking.Application.DTOs.LeaveRequestDTOs;

namespace LeaveTracking.Application.Validators
{
    public class LeaveRequestValidator : AbstractValidator<CreateLeaveRequestDto>
    {
        public LeaveRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Kullanıcı boş olamaz.")
                .GreaterThan(0).WithMessage("Kullanıcı Id'yi kontrol ediniz.");
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Baslangıç tarihi boş olamaz.")
                .LessThanOrEqualTo(x => x.EndDate).WithMessage("Baslangıç tarihi bitiş tarihinden sonra olmalıdır.");
            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("Bitis tarihi bos olamaz")
                .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("Bitiş tarihi baslangıç tarihinden önce olmalıdır.");
            RuleFor(x => x.LeaveType)
                .IsInEnum().WithMessage("Geçersiz izin.");
        }
    }
}