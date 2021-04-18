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
        
        public IActionResult Authenticate()
        {
            return Redirect("Index");
        }
        
        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }
    }
}