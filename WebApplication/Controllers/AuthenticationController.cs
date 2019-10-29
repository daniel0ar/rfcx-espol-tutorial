using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApplication.Services;
using WebApplication.Models;
using WebApplication.IRepository;
using WebApplication.Repository;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace WebApplication.Controllers
{
    [Authorize]
        public class AuthenticationController : Controller
        {
            [AllowAnonymous]
            public IActionResult Index()
            {
                return View();
            }

            private IAuthService _authService;

            public AuthenticationController(IAuthService authService)
            {
                _authService = authService;
            }

            [AllowAnonymous]
            [HttpPost("[controller]")]
            public async Task<IActionResult> Authenticate(string Username, string Password)
            {
                var user = _authService.Authenticate(Username, Password);
                if (user == null)
                    return BadRequest(new { message = "Username or password is incorrect" });

                CookieOptions option = new CookieOptions();
                //Cookie expiration time
                option.Expires = System.DateTime.Now.AddMinutes(20);
                Response.Cookies.Append("token", user.Token, option);

                return Redirect("/Home");
            }

            [AllowAnonymous]
            [HttpPost("api/[controller]/token")]
            public async Task<IActionResult> token([FromBody]User userParam)
            {
                var user = _authService.Authenticate(userParam.Username, userParam.Password);

                if (user == null)
                    return BadRequest(new { message = "Username or password is incorrect" });

                return Ok(JsonConvert.SerializeObject(new {token = user.Token}));
            }
        }
}