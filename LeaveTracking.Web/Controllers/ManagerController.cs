using LeaveTracking.Application.DTOs.LeaveRequestDTOs;
using LeaveTracking.Application.DTOs.UserDTOs;
using LeaveTracking.Application.DTOs.WebDTOs;
using LeaveTracking.Application.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace LeaveTracking.Web.Controllers
{
    public class ManagerController : BaseController
    {
        public ManagerController(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<IActionResult> Index()
        {
            if (GetUserRole() == "Employee")
            {
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                AddJwtToken();
                var response = await _httpClient.GetAsync("/api/manager/dash");
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Veriler alınamadı.";
                    return View(new ManagerDash());
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var result = JsonSerializer.Deserialize<ResponseResult<ManagerDash>>(responseContent, options);

                return View(result?.Data);
            }
        }

        public async Task<IActionResult> Register(string? errormessage, string? successmessage)
        {
            if (GetUserRole() == "Employee")
            {
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                AddJwtToken();
                var response = await _httpClient.GetAsync("/api/users"); //degis
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Veriler alınamadı.";
                    return View(new List<ResultUserDto>());
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var result = JsonSerializer.Deserialize<ResponseResult<List<ResultUserDto>>>(responseContent, options);
                ViewBag.Users = result?.Data;
                if (!string.IsNullOrEmpty(errormessage))
                    ViewBag.Error = errormessage;
                if (!string.IsNullOrEmpty(successmessage))
                    ViewBag.Success = successmessage;
                ViewBag.TotalUsers = result?.Data.Count();
                ViewBag.ManagerCount = result?.Data.Count(x => x.Role == Domain.Enums.UserRole.Manager);
                ViewBag.EmployeeCount = result?.Data.Count(x => x.Role == Domain.Enums.UserRole.Employee);
                ViewBag.LowQuotaUsers = result?.Data.Count(x => x.AnnualLeaveQuota < 5);
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (GetUserRole() == "Employee")
            {
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                if (!ModelState.IsValid)
                    return View(model);
                AddJwtToken();
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/user", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<ResponseResult<string>>(responseContent, options);
                if (!result.Success)
                {
                    ViewBag.Error = result.Message;
                    return RedirectToAction("Register", "Manager", new { errormessage = result.Message });
                }
                return RedirectToAction("Register", "Manager", new { successmessage = "Kullanıcı başarılı bir şekilde eklendi." });
            }
        }

        public async Task<IActionResult> LeaveRequest()
        {
            if (GetUserRole() == "Employee")
            {
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                AddJwtToken();
                var response = await _httpClient.GetAsync("/api/leaverequest");
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Veriler alınamadı.";
                    return View(new List<LeaveRequestDto>());
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var result = JsonSerializer.Deserialize<ResponseResult<List<LeaveRequestDto>>>(responseContent, options);

                return View(result?.Data);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApproveLeave(int id, int page = 1)
        {
            if (GetUserRole() == "Employee")
            {
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                AddJwtToken();
                var response = await _httpClient.PostAsync("/api/leaverequest/approve?id=" + id, null);
                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<ResponseResult<string>>(responseContent, options);
                if (!result.Success)
                {
                    ViewBag.Error = result.Message;
                    return View();
                }
                if (page == 1)
                {
                    return RedirectToAction("LeaveRequest", "Manager");
                }
                else
                {
                    return RedirectToAction("CalendarLeaveRequest", "Manager");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectLeave(RejectLeaveRequestDto model, int page = 1)
        {
            if (GetUserRole() == "Employee")
            {
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                AddJwtToken();
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/leaverequest/reject", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<ResponseResult<string>>(responseContent, options);
                if (page == 1)
                {
                    return RedirectToAction("LeaveRequest", "Manager");
                }
                else
                {
                    return RedirectToAction("CalendarLeaveRequest", "Manager");
                }
            }
        }

        public async Task<IActionResult> CalendarLeaveRequest()
        {
            if (GetUserRole() == "Employee")
            {
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                AddJwtToken();
                var response = await _httpClient.GetAsync("/api/leaveRequest");
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Veriler alınamadı.";
                    return View(new List<LeaveRequestDto>());
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var result = JsonSerializer.Deserialize<ResponseResult<List<LeaveRequestDto>>>(responseContent, options);

                return View(result?.Data);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string email)
        {
            if (GetUserRole() == "Employee")
            {
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                AddJwtToken();
                var response = await _httpClient.DeleteAsync("/api/user?email=" + email);
                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<ResponseResult<bool>>(responseContent, options);
                if (!result.Success)
                {
                    ViewBag.Error = result.Message;
                    return RedirectToAction("Register", "Manager", new { errormessage = result.Message });
                }
                return RedirectToAction("Register", "Manager", new { successmessage = "Kullanıcı başarılı bir şekilde silindi." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendAnnualSummary()
        {
            if (GetUserRole() == "Employee")
            {
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                AddJwtToken();
                var response = await _httpClient.PostAsync("/api/user/annualleavequota", null);
                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<ResponseResult<bool>>(responseContent, options);
                if (!result.Success)
                {
                    return Json(new { success = false, message = result.Message });
                }
                return Json(new { success = true, message = "Başarılı!" });
            }
        }
    }
}