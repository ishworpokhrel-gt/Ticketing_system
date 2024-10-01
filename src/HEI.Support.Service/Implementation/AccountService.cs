using HEI.Support.Common.Models;
using HEI.Support.Domain.Entities;
using HEI.Support.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Encodings.Web;

namespace HEI.Support.Service.Implementation
{
    public class AccountService : IAccountService
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<LoginViewModel> _logger;
		private readonly IEmailService _emailSender;
		private LinkGenerator _linkGenerator;
		private readonly IHttpContextAccessor _httpContextAccessor;


		public AccountService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IEmailService emailSender, ILogger<LoginViewModel> logger, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_logger = logger;
			_emailSender = emailSender;
			_linkGenerator = linkGenerator;
			_httpContextAccessor = httpContextAccessor;
		}

        public async Task<(int status, string message)> AuthenticateUser(LoginViewModel login)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(login.UserName);

            if (user == null)
            {
                return (-1, "Invalid username or password.");
            }

            var result = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, login.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation($"User {login.UserName} logged in successfully.");
                user.LastLoginTime = DateTime.UtcNow;
                await _signInManager.UserManager.UpdateAsync(user);
                return (1, "success");
            }

            // Reload user to get the latest AccessFailedCount
            user = await _signInManager.UserManager.FindByNameAsync(login.UserName);

            if (result.IsLockedOut)
            {
                _logger.LogWarning($"User account {login.UserName} locked out.");
                return (3, "Account is locked out.");
            }

            int failedAttempts = await _signInManager.UserManager.GetAccessFailedCountAsync(user);
            int maxAttempt = _signInManager.UserManager.Options.Lockout.MaxFailedAccessAttempts;
            int remainingAttempts = maxAttempt - failedAttempts;

            _logger.LogWarning($"Invalid login attempt for user {login.UserName}. Total remaining attempts: {remainingAttempts}.");
            return (4, $"Invalid login attempt. Total remaining attempts: {remainingAttempts}.");
        }



        public async Task<(int status, string message)> RegisterUser(RegisterViewModel userData)
        {
            var user = new ApplicationUser
            {
                UserName = userData.Email,
                Email = userData.Email,
                RegistrationDate = DateTime.UtcNow,
                LastLoginTime = null,
                LastLogoutTime = null  // Optional, initialize to null
            };

            var result = await _userManager.CreateAsync(user, userData.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                // Assign default role
                var role = "EndUser";
                var roleResult = await _userManager.AddToRoleAsync(user, role);

                if (!roleResult.Succeeded)
                {
                    var errorMessage = string.Join("~", roleResult.Errors.Select(error => error.Description));
                    return (-1, errorMessage);
                }

                // Send email confirmation
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var protocol = _httpContextAccessor.HttpContext.Request.Scheme;
                var host = _httpContextAccessor.HttpContext.Request.Host;

                var routeValues = new
                {
                    userId = userId,
                    code = code
                };

                var url = _linkGenerator.GetPathByAction("ConfirmEmail", "Account", values: routeValues);
                var completeUrl = $"{protocol}://{host}{url}";

                await _emailSender.SendMailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(completeUrl)}'>clicking here</a>.");

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return (1, "RegisterConfirmationPage");
                }
                else
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return (2, "Registration successful.");
                }
            }
            else
            {
                var errorMessage = string.Join("~", result.Errors.Select(error => error.Description));
                return (-1, errorMessage);
            }
        }


        public async Task<(int status, string message)> HandleForgotPassword(string Email)
		{
			var protocol = _httpContextAccessor.HttpContext.Request.Scheme;
			var host = _httpContextAccessor.HttpContext.Request.Host;

			var user = await _userManager.FindByEmailAsync(Email);
			if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
			{
				// Don't reveal that the user does not exist or is not confirmed
				return (-1, "ForgotPasswordConfirmation");
			}

			var code = await _userManager.GeneratePasswordResetTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

			var url = _linkGenerator.GetPathByAction("ResetPassword", "Account", new { code });
			var completeUrl = $"{protocol}://{host}{url}";
			await _emailSender.SendMailAsync(
				Email,
				"Reset Password",
				$"Please reset your password by <a href='{HtmlEncoder.Default.Encode(completeUrl)}'>clicking here</a>.");
			return (1, "success");
		}

		public async Task<(int status, string message)> HandleChangePassword(ChangePasswordViewModel ChangePasswordViewModel, ApplicationUser user)
		{
			if (user == null)
			{
				return (0, "Unable to load user with ID.");
			}
			var changePasswordResult = await _userManager.ChangePasswordAsync(user, ChangePasswordViewModel.CurrentPassword, ChangePasswordViewModel.NewPassword);
			if (!changePasswordResult.Succeeded)
			{
				var errorMessage = String.Empty;
				foreach (var error in changePasswordResult.Errors)
				{
					errorMessage = errorMessage + '~' + error.Description;
				}
				return (-1, errorMessage);
			}

			await _signInManager.RefreshSignInAsync(user);
			_logger.LogInformation("User changed their password successfully.");
			await _signInManager.SignOutAsync();
			return (1, "success");
		}
	}
}
