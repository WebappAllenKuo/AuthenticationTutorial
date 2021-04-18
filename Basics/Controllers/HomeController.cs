using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Basics.Controllers
{
    public class HomeController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<IActionResult> Authenticate()
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, "Allen")
            };
            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);
            Console.WriteLine("==== Home / Authencate =====");
            return Redirect("Index");
        }
        
        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }
    }
}