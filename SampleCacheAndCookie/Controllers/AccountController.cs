using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SampleCacheAndCookie.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : Controller
    {
        public IActionResult Login() 
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name ,"test"),
                new Claim(ClaimTypes.Role, "Administrator"),
                new Claim(ClaimTypes.SerialNumber, Guid.NewGuid().ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                RedirectUri = Url.Action("Index", "Home")
            };

            return SignIn(claimsPrincipal, authProperties, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public IActionResult Logout()
        {
            var authProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home")
            };

            return SignOut(authProperties, CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
