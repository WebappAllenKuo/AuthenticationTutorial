using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IdentityExample.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using NETCore.MailKit.Core;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            // 測試成功，暫時 mark掉
            // await _emailService.SendAsync("allen@test.com", "subject", "body");

            return View();
            
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string userName, string email, string password)
        {
            var newUser = new IdentityUser
            {
                UserName = userName,
                Email = email
            };
            var result =await _userManager.CreateAsync(newUser, password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty,"regsiter fail");
                AddErrors(result.Errors);
                return View();
            }
            
            // // sign in
            // var user = await _userManager.FindByNameAsync(userName);
            // ProcessSignIn(user, password);
            //
            // return RedirectToAction("Index");
            
            // 寄送 verify email
            // todo
            
            return RedirectToAction("EmailVerification");
        }

        private void AddErrors(IEnumerable<IdentityError> resultErrors)
        {
            string msg = resultErrors.Select(x => x.Description)
                    .Aggregate((acc, next) => acc += next)
                ;
            _logger.LogWarning(msg,null);
        }

        [Authorize]
        public IActionResult Secret() => View();
        
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty,"user not found");
                return View();
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (result.Succeeded)
            {
                // sign
                await ProcessSignIn(user, password);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty,"password error");
                return View();
            }
        }

        private async Task ProcessSignIn(IdentityUser user, string password)
        {
            var result =await _signInManager.PasswordSignInAsync(user, password, false, false);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, new AuthenticationProperties(null));
            }
        }

        public async Task<IActionResult> Logout()
        {
           await _signInManager.SignOutAsync();
           return RedirectToAction("Index");
        }

        public IActionResult EmailVerification() => View();
    }
}