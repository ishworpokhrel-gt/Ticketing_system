using HEI.Support.Areas.Admin.Models;
using HEI.Support.Data.Entities;
using HEI.Support.Service.Implementation;
using HEI.Support.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HEI.Support.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly SignInManager<ApplicationUser> _signInmanager;
        public AccountController(IAccountService accountService, UserManager<ApplicationUser> usermanager, SignInManager<ApplicationUser> signInmanager)
        {
            _accountService = accountService;
            _usermanager = usermanager;
            _signInmanager = signInmanager;
        }

        public IActionResult Login()
        {
            return View();
        }

        // Login POST
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (!ModelState.IsValid)
                return View(loginModel);

            var result = await _accountService.LoginAsync(loginModel);
            if (!result.IsSucceed)
            {
                ModelState.AddModelError("", result.Message);
                return View(loginModel);
            }

            return RedirectToAction("Index", "Home"); // Redirect to home or another page after successful login
        }

        // Register GET
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Register POST
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            if (!ModelState.IsValid)
                return View(registerModel);

            var result = await _accountService.RegisterAsync(registerModel);
            if (!result.IsSucceed)
            {
                ModelState.AddModelError("", result.Message);
                return View(registerModel);
            }

            return RedirectToAction("Login");
        }

        // Forgot Password GET
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Forgot Password POST
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.ForgotPasswordAsync(model);
            if (!result.IsSucceed)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            ViewBag.Message = "Password reset link sent successfully.";
            return View();
        }

        // Reset Password GET
        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View(new ResetPasswordViewModel { Code = code });
        }

        // Reset Password POST
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.ResetPasswordAsync(model);
            if (!result.IsSucceed)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            return RedirectToAction("Login");
        }

        // Change Password GET
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // Change Password POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _usermanager.GetUserAsync(User);
            var result = await _accountService.ChangePasswordAsync(model, user);

            if (!result.IsSucceed)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            return RedirectToAction("Login");
        }

        // Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInmanager.SignOutAsync(); // Implement SignOutAsync in AccountService
            return RedirectToAction("Login");
        }
    }
}


