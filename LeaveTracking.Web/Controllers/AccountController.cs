using LeaveTracking.Application.DTOs.UserDTOs;
using LeaveTracking.Application.Shared;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

namespace LeaveTracking.Web.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ResponseResult<string>>(responseContent, options);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }

            if (!result.Success)
            {
                ViewBag.Error = "Token alınamadı.";
                return View(model);
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(1),
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("JWToken", result.Data, cookieOptions);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(result.Data);
            var role = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            if (role == "Manager")
            {
                return RedirectToAction("Index", "Manager");
            }
            if (role == "Employee")
            {
                return RedirectToAction("Index", "Employee");
            }
            ViewBag.Error = "Hata oluştu.";
            return View(model);
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("JWToken");
            return RedirectToAction("Login", "Account");
        }
    }
}