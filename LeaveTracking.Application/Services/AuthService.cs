using AutoMapper;
using FluentValidation;
using LeaveTracking.Application.DTOs.UserDTOs;
using LeaveTracking.Application.Interfaces.Repositories;
using LeaveTracking.Application.Interfaces.Services;
using LeaveTracking.Application.Shared;
using LeaveTracking.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LeaveTracking.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IValidator<RegisterDto> _userValidator;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, IValidator<RegisterDto> userValidator, IMapper mapper, ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _userValidator = userValidator;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<ResponseResult<string>> Login(LoginDto dto)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(dto.Email);
                if (user == null)
                    return ResponseResult<string>.Failure("NOT_FOUND", "Kullanıcı bulunamadı.");
                var validatepassword = await ValidatePassword(dto.Password, user.PasswordHash);
                if (!validatepassword)
                    return ResponseResult<string>.Failure("INVALID_PASSWORD", "Kullanıcı emaili ve şifreyi kontrol edin.");

                var token = GenerateToken(user);

                if (token != null)
                {
                    return ResponseResult<string>.SuccessResult(token);
                }
                else
                {
                    return ResponseResult<string>.Failure("FAILED", "Giriş başarısız, yöneticiye başvurun.");
                }
            }
            catch (Exception ex)
            {
                return ResponseResult<string>.Failure("EXCEPTION", "Bir hata oluştu.");
            }
        }

        public async Task<ResponseResult<string>> Register(RegisterDto dto)
        {
            try
            {
                var validate = await _userValidator.ValidateAsync(dto);
                if (!validate.IsValid)
                    return ResponseResult<string>.Failure("VALIDATION_ERROR", validate.Errors.Select(x => x.ErrorMessage).FirstOrDefault());

                var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
                if (existingUser != null)
                    return ResponseResult<string>.Failure("USER_EXISTS", "Bu e-posta ile zaten bir kullanıcı var.");

                var user = _mapper.Map<User>(dto);
                user.PasswordHash = HashPassword(dto.Password);
                user.CreatedBy = _currentUserService.UserId;
                await _userRepository.CreateUserAsync(user);
                return ResponseResult<string>.SuccessResult("TRUE");
            }
            catch (Exception ex)
            {
                return ResponseResult<string>.Failure("EXCEPTION", "Bir hata oluştu.");
            }
        }

        private async Task<bool> ValidatePassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}