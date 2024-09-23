using HEI.Support.Common.Models;
using HEI.Support.Domain.Entities;

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