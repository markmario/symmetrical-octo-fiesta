using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using YourNamespace.Models;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login([FromQuery] string username, [FromQuery] string password)
        {
            var users = _configuration.GetSection("SwaggerCredentials:Users")
                                    .Get<List<SwaggerUser>>();

            var user = users?.FirstOrDefault(u => 
                u.Username == username && 
                u.Password == password);

            if (user != null)
            {
                var claims = new[] {
                    new Claim(ClaimTypes.Name, user.Username)
                };

                var identity = new ClaimsIdentity(claims, "Cookies");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("Cookies", principal);
                
                return Redirect("/swagger/index.html");
            }

            return Unauthorized();
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            
            return Ok("Logged Out. Sign in again to access the swagger page");
        }
    }
} 