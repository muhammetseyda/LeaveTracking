using LeaveTracking.Application.DTOs.LeaveRequestDTOs;
using LeaveTracking.Application.DTOs.WebDTOs;
using LeaveTracking.Application.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace LeaveTracking.Web.Controllers
{
    public class EmployeeController : BaseController
    {
        public EmployeeController(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<IActionResult> Index()
        {
            if (GetUserRole() == "Manager")
            {
                return RedirectToAction("Index", "Manager");
            }
            else
            {
                AddJwtToken();
                var response = await _httpClient.GetAsync("/api/employee/dash");
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Veriler alınamadı.";
                    return View(new EmployeeDash());
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var result = JsonSerializer.Deserialize<ResponseResult<EmployeeDash>>(responseContent, options);

                var user = GetUserName();
                ViewBag.Name = user;
                return View(result?.Data);
            }
        }

        public IActionResult CreateLeave(string? errormessage, string? successmessage)
        {
            if (!string.IsNullOrEmpty(errormessage))
                ViewBag.Error = errormessage;
            if (!string.IsNullOrEmpty(successmessage))
                ViewBag.Success = successmessage;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateLeaveRequest(CreateLeaveRequestDto model)
        {
            AddJwtToken();
            var userId = GetUserId();
            if (userId == 0)
            {
                ViewBag.Error = "Yetkisiz giris.";
                return RedirectToAction("CreateLeave", "Employe", new { errormessage = "Yetkisiz giris." });
            }
            else
            {
                model.UserId = userId;
            }
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/leaveRequest", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ResponseResult<string>>(responseContent, options);
            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                return RedirectToAction("CreateLeave", "Employee", new { errormessage = result.Message });
            }
            return RedirectToAction("CreateLeave", "Employee", new { successmessage = "Talep başarılı bir şekilde oluşturuldu." });
        }

        public async Task<IActionResult> LeaveRequest(string? errormessage, string? successmessage)
        {
            AddJwtToken();
            var userId = GetUserId();
            var response = await _httpClient.GetAsync("/api/leaverequest/user?userId=" + userId);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Veriler alınamadı.";
                return View(new List<LeaveRequestDto>());
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var result = JsonSerializer.Deserialize<ResponseResult<List<LeaveRequestDto>>>(responseContent, options);

            if (!string.IsNullOrEmpty(errormessage))
                ViewBag.Error = errormessage;
            if (!string.IsNullOrEmpty(successmessage))
                ViewBag.Success = successmessage;
            return View(result?.Data);
        }

        public async Task<IActionResult> DeleteLeaveRequest(int id)
        {
            AddJwtToken();
            var response = await _httpClient.DeleteAsync("/api/leaverequest?id=" + id);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Talep silinemedi.";
                return RedirectToAction("LeaveRequest", "Employee", new { errormessage = "Talep silinemedi." });
            }
            return RedirectToAction("LeaveRequest", "Employee", new { successmessage = "Talep başarılı bir şekilde silindi." });
        }
    }
}