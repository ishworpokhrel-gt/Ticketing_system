using HEI.Support.Areas.Admin.Models;

namespace HEI.Support.Service.Implementation
{
    public interface IUserManagementService
    {
        Task<List<UserViewModel>> GetAllUsersAsync();
        Task<EditUserViewModel> GetUserByIdAsync(string id);
        Task<bool> UpdateUserAsync(EditUserViewModel model);
        Task<bool> DisableUserAsync(string id);
        Task<bool> EnableUserAsync(string id);
        Task<bool> ChangeUserRolesAsync(string userId, List<string> selectedRoles);
        Task<List<string>> GetUserRolesAsync(string userId);
        Task<List<string>> GetAllRolesAsync();
        Task<(bool status, string message)> RegisterUserAsync(RegisterUserViewModel model);
    }
}