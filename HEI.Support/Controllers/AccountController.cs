using HEI.Support.Areas.Admin.Models;
using HEI.Support.Data.Entities;
using HEI.Support.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace HEI.Support.Controllers
{
	public class AccountController : Controller
	{
		private readonly IAccountService _accountService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		public AccountController(IAccountService accountService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			_accountService = accountService;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public IActionResult Login()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel login)
		{
			(int status, string message) = await _accountService.AuthenticateUser(login);

			switch (status)
			{
				case -1:        //login failed
					ModelState.AddModelError(string.Empty, message);
					return View();

				case 1:     //success
					return RedirectToAction(nameof(HomeController.Index), "Home");

				case 2:     //Email not verified
					ModelState.AddModelError(string.Empty, message);
					return View();

				case 3:     //Lockout
					ModelState.AddModelError(string.Empty, message);
					return View("Lockout");

				case 4:     //Invalid Login Attempt
					ModelState.AddModelError(string.Empty, message);
					return View();
				default:
					return View();
			}
		}

		public ActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> Register(RegisterViewModel register)
		{
			(int status, string message) = await _accountService.RegisterUser(register);
			switch (status)
			{
				case 1: //email register confirmation page
					return RedirectToAction("RegisterConfirmation");

				case 2:     //register successful
					return RedirectToAction("Login");

				case -1:     //error during registration
					List<string> errorList = SplitStringByDelimiter(message, '~');
					foreach (string error in errorList)
					{
						ModelState.AddModelError(string.Empty, error);
					}
					return View();

				default:
					return View();
			}
		}

		public IActionResult RegisterConfirmation()
		{
			return View();
		}

		public async Task<IActionResult> LogOut()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Login");
		}

		public IActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ForgotPassword(ForgetPasswordViewModel ForgetPasswordViewModel)
		{
			if (ModelState.IsValid)
			{
				(int status, string message) = await _accountService.HandleForgotPassword(ForgetPasswordViewModel.Email);

				switch (status)
				{
					case 1:
						return View("ForgotPasswordConfirmation");

					case -1:
						return View("ForgotPasswordConfirmation");

					default:
						return View();
				}
			}
			return View();
		}

		public IActionResult ChangePassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel ChangePasswordViewModel)
		{
			var user = await _userManager.GetUserAsync(User);
			if (!ModelState.IsValid)
			{
				return View();
			}
			(int status, string message) = await _accountService.HandleChangePassword(ChangePasswordViewModel, user);
			switch (status)
			{
				case 0:  //user not found
					return NotFound(message);

				case 1:     //success
					return View("ChangePasswordConfirmation");

				case -1:   //error
					List<string> errorList = SplitStringByDelimiter(message, '~');
					foreach (string error in errorList)
					{
						ModelState.AddModelError(string.Empty, error);
					}
					return View();

				default:
					return View();
			}
		}

		public IActionResult ResetPassword(string code = null)
		{
			ResetPasswordViewModel Input;
			if (code == null)
			{
				return BadRequest("A code must be supplied for password reset.");
			}
			else
			{
				Input = new ResetPasswordViewModel
				{
					Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
				};
			}
			return View(Input);
		}

		[HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel ResetPasswordViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			var user = await _userManager.FindByEmailAsync(ResetPasswordViewModel.UserName);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return View("ResetPasswordConfirmation");
			}

			var result = await _userManager.ResetPasswordAsync(user, ResetPasswordViewModel.Code, ResetPasswordViewModel.Password);
			if (result.Succeeded)
			{
				return View("Login");
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
			return View();
		}


		[HttpGet]
		public async Task<IActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return RedirectToAction("Register");
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{userId}'.");
			}

			code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
			var result = await _userManager.ConfirmEmailAsync(user, code);
			bool Status = result.Succeeded ? true : false;
			return View("~/Views/Account/ConfirmEmailStatus.cshtml", Status);
		}

		public IActionResult AccessDenied()
		{
			return View();
		}
		List<string> SplitStringByDelimiter(string input, char delimiter)
		{
			string[] splitArray = input.Split(delimiter);
			List<string> resultList = new List<string>(splitArray);
			return resultList;
		}
	}
}


