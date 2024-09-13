using HEI.Support.Areas.Admin.Models;
using HEI.Support.Data.Entities;
using HEI.Support.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace HEI.Support.Service.Interface
{
    public interface IAccountService
    {
        Task<ResponseViewModel> SeedRolesAsync();
        Task<ResponseViewModel> RegisterAsync(RegisterViewModel model);
        Task<ResponseViewModel> LoginAsync(LoginViewModel model);
        Task<ResponseViewModel> ForgotPasswordAsync(ForgetPasswordViewModel model);
        Task<ResponseViewModel> ResetPasswordAsync(ResetPasswordViewModel model);
        Task<ResponseViewModel> ChangePasswordAsync(ChangePassword changePassword, ApplicationUser user);
        Task<ResponseViewModel> RegisterEmployeeAsync(RegisterViewModel model);
    }
}
