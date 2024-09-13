﻿using HEI.Support.Areas.Admin.Models;
using HEI.Support.Data.Entities;

namespace HEI.Support.Service.Interface
{
    public interface IAccountService
    {
        Task<ResponseViewModel> SeedRolesAsync();
        Task<ResponseViewModel> RegisterAsync(RegisterViewModel model);
        Task<ResponseViewModel> LoginAsync(LoginViewModel model);
        Task<ResponseViewModel> ForgotPasswordAsync(ForgetPasswordViewModel model);
        Task<ResponseViewModel> ResetPasswordAsync(ResetPasswordViewModel model);
        Task<ResponseViewModel> ChangePasswordAsync(ChangePasswordViewModel changePassword, ApplicationUser user);
        Task<ResponseViewModel> RegisterEmployeeAsync(RegisterViewModel model);
    }
}
