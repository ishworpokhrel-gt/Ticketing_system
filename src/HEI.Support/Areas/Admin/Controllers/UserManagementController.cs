using HEI.Support.Common.Models;
using HEI.Support.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HEI.Support.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly IUserManagementService _userManagementService;

        public UserManagementController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManagementService.GetAllUsersAsync();
            return View(users);
        }
        public async Task<IActionResult> RegisterUser()
        {
            var allRoles = await _userManagementService.GetAllRolesAsync();
            var filteredRoles = allRoles
                .Where(role => role == "EndUser" || role == "Support")
                .Select(role => new SelectListItem
                {
                    Value = role,
                    Text = role
                })
                .ToList();
            var orderedRoles = filteredRoles
                .OrderBy(role => role.Text == "EndUser" ? 0 : 1)
                .ToList();
            var model = new RegisterUserViewModel
            {
                AvailableRoles = orderedRoles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel model)
        {
            if (!ModelState.IsValid)
            {

				var allRoles = await _userManagementService.GetAllRolesAsync();
				var filteredRoles = allRoles
					.Where(role => role == "EndUser" || role == "Support")
					.Select(role => new SelectListItem
					{
						Value = role,
						Text = role
					})
					.ToList();
				var orderedRoles = filteredRoles
					.OrderBy(role => role.Text == "EndUser" ? 0 : 1)
					.ToList();
				var modelView = new RegisterUserViewModel
				{
					AvailableRoles = orderedRoles
				};

				return View(modelView);
            }

            (bool status, string message) = await _userManagementService.RegisterUserAsync(model);

            switch (status)
            {
                case true:     //register successful
                    TempData["Message"] = "User registered successfully.";
                    return RedirectToAction("Index");

                case false:     //error during registration
                    List<string> errorList = SplitStringByDelimiter(message, '~');
                    foreach (string error in errorList)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    TempData["Message"] = message;
                    TempData["IsSuccess"] = false;
                    var allRoles = await _userManagementService.GetAllRolesAsync();
                    var filteredRoles = allRoles
                        .Where(role => role == "EndUser" || role == "Support")
                        .Select(role => new SelectListItem
                        {
                            Value = role,
                            Text = role
                        })
                        .ToList();
                    var orderedRoles = filteredRoles
                        .OrderBy(role => role.Text == "EndUser" ? 0 : 1)
                        .ToList();
                    var modelView = new RegisterUserViewModel
                    {
                        AvailableRoles = orderedRoles
                    };

                    return View(modelView);
            }
        }
        List<string> SplitStringByDelimiter(string input, char delimiter)
        {
            string[] splitArray = input.Split(delimiter);
            List<string> resultList = new List<string>(splitArray);
            return resultList;
        }
        public async Task<IActionResult> EditUser(string id)
        {
            var model = await _userManagementService.GetUserByIdAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _userManagementService.UpdateUserAsync(model);
            if (result)
            {
                TempData["Message"] = "User updated successfully.";
                TempData["IsSuccess"] = true;
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Error updating user.");
            TempData["IsSuccess"] = false;
            TempData["Message"] = "Error updating user.";
            return View(model);
        }

        public async Task<IActionResult> DisableUser(string id)
        {
            var result = await _userManagementService.DisableUserAsync(id);
            TempData["IsSuccess"] = result;
            TempData["Message"] = result ? "User has been disabled." : "Error disabling user.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EnableUser(string id)
        {
            var result = await _userManagementService.EnableUserAsync(id);
            TempData["IsSuccess"] = result;
            TempData["Message"] = result ? "User has been enabled." : "Error enabling user.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ChangeRoleUser(string id)
        {
            var user = await _userManagementService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManagementService.GetUserRolesAsync(user.Id);
            var allRoles = await _userManagementService.GetAllRolesAsync();

            var model = new ChangeRoleViewModel
            {
                UserId = user.Id,
                CurrentRoles = roles,
                AllRoles = allRoles
            };

            return View(model);
        }

		[HttpPost]
		public async Task<IActionResult> ChangeUserRole(ChangeRoleViewModel model)
		{
			if (ModelState.IsValid)
			{
				var success = await _userManagementService.ChangeUserRolesAsync(model.UserId, model.SelectedRole);
				TempData["IsSuccess"] = success;
				TempData["Message"] = success ? "User roles updated successfully." : "Error updating roles.";
			}
			else
			{
				TempData["IsSuccess"] = false;
				TempData["Message"] = "Invalid data submitted.";
			}

			return RedirectToAction("Index");
		}


		public async Task<IActionResult> DetailsUser(string id)
        {
            var user = await _userManagementService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }
    }
}
