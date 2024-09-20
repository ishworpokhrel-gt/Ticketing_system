﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text;
using HEI.Support.Service.Interface;
using HEI.Support.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using HEI.Support.Common.Models;

namespace HEI.Support.Service.Implementation
{
    public class UserManagementService : IUserManagementService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserManagementService> _logger;
        private LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailSender;


        public UserManagementService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<UserManagementService> logger, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor, IEmailService emailSender, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;
            _signInManager = signInManager;
        }
        public async Task<(bool status, string message)> RegisterUserAsync(RegisterUserViewModel model)
        {
            string password = GenerateRandomPassword(12);

            // Create a new user object
            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.UserName,
                PhoneNumber = model.PhoneNumber,
                TelNumber = model.TelNumber,
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                // Assign default role
                var role = "EndUser";
                var roleResult = await _userManager.AddToRoleAsync(user, role);

                if (!roleResult.Succeeded)
                {
                    var errorMessage = string.Join("~", roleResult.Errors.Select(error => error.Description));
                    return (false, errorMessage);
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

                var email = await _emailSender.SendMailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(completeUrl)}'>clicking here</a>.");

                await _signInManager.SignInAsync(user, isPersistent: false);
                return (true, "Registration successful.");
            }
            else
            {
                var errorMessage = string.Join("~", result.Errors.Select(error => error.Description));
                return (false, errorMessage);
            }
        }

        private string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[length];
                rng.GetBytes(bytes);
                return new string(Enumerable.Range(0, length).Select(i => validChars[bytes[i] % validChars.Length]).ToArray());
            }
        }

        public async Task<List<UserViewModel>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                LogUserDetails(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    IsLockedOut = user.LockoutEnd.HasValue &&
                                  user.LockoutEnd > DateTimeOffset.UtcNow,
                    //RegistrationDate = IsValidDate(user.RegistrationDate) ? user.RegistrationDate : null,
                    //LastLoginTime = IsValidDate(user.LastLoginTime) ? user.LastLoginTime : null,
                    //LastLogoutTime = IsValidDate(user.LastLogoutTime) ? user.LastLogoutTime : null,
                    FailedLoginAttempts = user.AccessFailedCount,
                    Roles = roles.ToList()
                });
            }

            return userViewModels;
        }

        private bool IsValidDate(DateTime? date)
        {
            if (date.HasValue)
            {
                if (date.Value.Year <= 0 || date.Value.Year >= 10000)
                {
                    return false; // Invalid year
                }
            }
            return true; // Valid or null date
        }

        private void LogUserDetails(ApplicationUser user)
        {
            if (!IsValidDate(user.RegistrationDate))
            {
                Console.WriteLine($"Invalid RegistrationDate for User ID {user.Id}: {user.RegistrationDate}");
            }
            if (!IsValidDate(user.LastLoginTime))
            {
                Console.WriteLine($"Invalid LastLoginTime for User ID {user.Id}: {user.LastLoginTime}");
            }
            if (!IsValidDate(user.LastLogoutTime))
            {
                Console.WriteLine($"Invalid LastLogoutTime for User ID {user.Id}: {user.LastLogoutTime}");
            }
        }

        public async Task<EditUserViewModel> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            return new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };
        }

        public async Task<bool> UpdateUserAsync(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return false;

            user.Email = model.Email;
            user.UserName = model.UserName;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DisableUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> EnableUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            user.LockoutEnd = null;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ChangeUserRolesAsync(string userId, List<string> selectedRoles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            var result = await _userManager.AddToRolesAsync(user, selectedRoles);
            return result.Succeeded;
        }
        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();

            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }
        public async Task<List<string>> GetAllRolesAsync()
        {
            var roles = new List<string>();
            var data = await _roleManager.Roles.Select(role => role.Name).ToListAsync();
            if (data is not null)
                roles = data;
            return roles;
        }
    }
}
