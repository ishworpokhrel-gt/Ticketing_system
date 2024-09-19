using HEI.Support.Areas.Admin.Models;
using HEI.Support.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace HEI.Support.Service.Interface
{
    public interface IAccountService
    {
        Task<(int status, string message)> AuthenticateUser(LoginViewModel login);
        Task<(int status, string message)> HandleChangePassword(ChangePasswordViewModel ChangePasswordViewModel, ApplicationUser user);
        Task<(int status, string message)> HandleForgotPassword(string Email);
        Task<(int status, string message)> RegisterUser(RegisterViewModel userData);
    }
}