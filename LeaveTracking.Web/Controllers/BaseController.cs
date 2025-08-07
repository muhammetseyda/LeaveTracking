using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace LeaveTracking.Web.Controllers
{
    public class BaseController : Controller
    {
        protected readonly HttpClient _httpClient;

        public BaseController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5271");
        }

        protected void AddJwtToken()
        {
            var token = HttpContext.Request.Cookies["JWToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        protected string GetUserRole()
        {
            var token = HttpContext.Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
                return null;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
        }

        protected int GetUserId()
        {
            var token = HttpContext.Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
                return 0;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var idClaim = jwt.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

            return int.TryParse(idClaim, out var userId) ? userId : 0;
        }

        protected bool CheckToken()
        {
            var token = HttpContext.Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
                return false;
            return true;
        }

        protected string GetUserName()
        {
            var token = HttpContext.Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
                return " ";

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var nameClaim = jwt.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;

            return string.IsNullOrEmpty(nameClaim) ? " " : nameClaim.ToString();
        }
    }
}