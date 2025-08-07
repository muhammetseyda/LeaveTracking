using FluentValidation;
using LeaveTracking.Application.DTOs.UserDTOs;

namespace LeaveTracking.Application.Validators
{
    public class UserRegisterValidator : AbstractValidator<RegisterDto>
    {
        public UserRegisterValidator()
        {
            RuleFor(u => u.FirstName)
                .NotEmpty().WithMessage("İsim zornludur.")
                .Length(2, 50).WithMessage("İsim 2 ile 50 karater arasında olmalıdır.");
            RuleFor(u => u.LastName)
                .NotEmpty().WithMessage("Soyisim zorunludur")
                .Length(2, 50).WithMessage("Soyisim 2 ile 50 karakter arasında olmalıdır.");
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email zorunludur.")
                .EmailAddress().WithMessage("Geçersiz email formatı.");
            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Şifre zorunludur.")
                .MinimumLength(6).WithMessage("Şifre 6 karakterden fazla olmalıdır.");
            RuleFor(u => u.Role)
                .IsInEnum().WithMessage("Geçersiz rol.");
        }
    }
}