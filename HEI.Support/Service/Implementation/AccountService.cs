using HEI.Support.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using HEI.Support.Areas.Admin.Models;
using HEI.Support.Models;
using HEI.Support.Service.Interface;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace HEI.Support.Service.Implementation
{
    public class AccountService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountService> _logger;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;

        public AccountService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ILogger<AccountService> logger, SignInManager<ApplicationUser> signInManager, IEmailService emailService, IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
            _signInManager = signInManager;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
        }



        public async Task<ResponseViewModel> LoginAsync(LoginViewModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.UserName);

            if (user is null)
                return new ResponseViewModel()
                {
                    IsSucceed = false,
                    Message = "Invalid Credentials"
                };

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginModel.Password);

            if (!isPasswordCorrect)
                return new ResponseViewModel()
                {
                    IsSucceed = false,
                    Message = "Invalid Credentials"
                };

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("JWTID", Guid.NewGuid().ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GenerateNewJsonWebToken(authClaims);

            return new ResponseViewModel()
            {
                IsSucceed = true,
                Message = "Login Successful."
            };
        }
        public async Task<ResponseViewModel> RegisterAsync(RegisterViewModel model)
        {
            var isExistsUser = await _userManager.FindByNameAsync(model.UserName);

            if (isExistsUser != null)
                return new ResponseViewModel()
                {
                    IsSucceed = false,
                    Message = "UserName Already Exists"
                };


            ApplicationUser newUser = new ApplicationUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var createUserResult = await _userManager.CreateAsync(newUser, model.Password);

            if (!createUserResult.Succeeded)
            {
                var errorString = "User Creation Failed Beacause: ";
                foreach (var error in createUserResult.Errors)
                {
                    errorString += " # " + error.Description;
                }
                return new ResponseViewModel()
                {
                    IsSucceed = false,
                    Message = errorString
                };
            }

            // Add a Default USER Role to all users

            foreach (var role in model.Roles)
            {
                await _userManager.AddToRoleAsync(newUser, role);
            }


            return new ResponseViewModel()
            {
                IsSucceed = true,
                Message = "User Created Successfully"
            };
        }
        public async Task<ResponseViewModel> ForgotPasswordAsync(ForgetPasswordViewModel model)
        {
            try
            {
                ResponseViewModel response = new ResponseViewModel { };
                string protocol = _httpContextAccessor.HttpContext.Request.Scheme;
                HostString host = _httpContextAccessor.HttpContext.Request.Host;


                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    if (user.Email != model.Email)
                    {
                        var userJson = JsonConvert.SerializeObject(user);
                        _logger.LogInformation($"UserName do not match with provided Email: {model.Email}. Provided UserName is {userJson}");
                        // Don't reveal that the user does not exist or is not confirmed
                        response.IsSucceed = false;
                        response.Message = "Something went Wrong.";

                    }
                    else
                    {
                        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        var url = _linkGenerator.GetPathByAction("ResetPassword", "Account", new { code });
                        var completeUrl = $"{protocol}://{host}{url}";
                        await _emailService.SendMailAsync(
                            model.Email,
                            "Reset Password",
                            $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(completeUrl)}'>clicking here</a>.");
                        response.IsSucceed = true;
                        response.Message = "Password Reset Mail Send Succesfully.";
                    }
                }
                else
                {
                    _logger.LogWarning("User Has not been Registered.");
                    response.IsSucceed = false;
                    response.Message = "User has not been Registered!";
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return (new ResponseViewModel
                {
                    IsSucceed = false,
                    Message = "Something went Worng " + ex.Message
                });
            }

        }
        public async Task<ResponseViewModel> ResetPasswordAsync(ResetPasswordViewModel resetPassword)
        {
            try
            {
                ResponseViewModel response = new ResponseViewModel { };
                var user = await _userManager.FindByNameAsync(resetPassword.UserName);
                if (user == null)
                {
                    _logger.LogInformation($"UserName is not registered {resetPassword.UserName}");
                    response.Message = "Something went Wrong";
                    response.IsSucceed = false;
                }
                else
                {
                    var result = await _userManager.ResetPasswordAsync(user, Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPassword.Code)), resetPassword.Password);
                    if (result.Succeeded)
                    {
                        response.Message = "Password Reset Successfully.";
                        response.IsSucceed = true;
                    }
                    else
                    {
                        response.Message = "Something went Wrong.";
                        response.IsSucceed = false;
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return new ResponseViewModel
                {
                    IsSucceed = false,
                    Message = "Something went Worng. " + ex.Message

                };
            }
        }
        public async Task<ResponseViewModel> ChangePasswordAsync(ChangePasswordViewModel changePassword, ApplicationUser user)
        {
            try
            {
                ResponseViewModel response = new ResponseViewModel { };
                if (user == null)
                {
                    response.Message = "Unable to load user with ID.";
                    response.IsSucceed = false;
                }
                else
                {
                    var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePassword.CurrentPassword, changePassword.NewPassword);
                    if (!changePasswordResult.Succeeded)
                    {
                        response.Message = "Something went Wrong.";
                        response.IsSucceed = false;
                        return response;
                    }
                    await _signInManager.RefreshSignInAsync(user);
                    _logger.LogInformation("User changed their password successfully.");
                    await _signInManager.SignOutAsync();
                    response.Message = "Password Changed Successfully.";
                    response.IsSucceed = true;
                }
                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return new ResponseViewModel
                {
                    IsSucceed = false,
                    Message = "Something went Worng. " + ex.Message
                };
            }
        }
        private string GenerateNewJsonWebToken(List<Claim> claims)
        {
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var tokenObject = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(1),
                    claims: claims,
                    signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

            return token;
        }


        public async Task<ResponseViewModel> RegisterEmployeeAsync(RegisterViewModel model)
        {
            var isExistsUser = await _userManager.FindByNameAsync(model.UserName);

            if (isExistsUser != null)
                return new ResponseViewModel()
                {
                    IsSucceed = false,
                    Message = "UserName Already Exists"
                };


            ApplicationUser newUser = new ApplicationUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var createUserResult = await _userManager.CreateAsync(newUser);

            if (!createUserResult.Succeeded)
            {
                var errorString = "User Creation Failed Beacause: ";
                foreach (var error in createUserResult.Errors)
                {
                    errorString += " # " + error.Description;
                }
                return new ResponseViewModel()
                {
                    IsSucceed = false,
                    Message = errorString
                };
            }

            // Add a Default USER Role to all users

            foreach (var role in model.Roles)
            {
                await _userManager.AddToRoleAsync(newUser, role);
            }


            return new ResponseViewModel()
            {
                IsSucceed = true,
                Message = "User Created Successfully"
            };
        }


    }
}
