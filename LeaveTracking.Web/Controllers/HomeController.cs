using LeaveTracking.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LeaveTracking.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (CheckToken())
            {
                var role = GetUserRole();
                if (role == "Manager")
                {
                    return RedirectToAction("Index", "Manager");
                }
                else if (role == "Employee")
                {
                    return RedirectToAction("Index", "Employee");
                }
            }
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}