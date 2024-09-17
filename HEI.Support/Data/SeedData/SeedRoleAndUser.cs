using HEI.Support.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace HEI.Support.Data.SeedData
{
	public class SeedRoleAndUser
	{
		public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager)
		{
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			string[] roleNames = { "Admin", "Support", "EndUser" };
			IdentityResult roleResult;

			// Create roles if they do not exist
			foreach (var roleName in roleNames)
			{
				var roleExist = await roleManager.RoleExistsAsync(roleName);
				if (!roleExist)
				{
					roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
					if (!roleResult.Succeeded)
					{
						// Handle role creation failure
						throw new Exception($"Failed to create role: {roleName}");
					}
				}
			}

			// Create an admin user if one does not exist
			ApplicationUser user = await userManager.FindByEmailAsync("admin@hei.com");
			if (user == null)
			{
				user = new ApplicationUser()
				{
					UserName = "admin@hei.com",
					Email = "admin@hei.com",
					FirstName = "Admin",
					LastName = "User"
				};
				var createUserResult = await userManager.CreateAsync(user, "Test@123");
				if (!createUserResult.Succeeded)
				{
					throw new Exception("Failed to create admin user.");
				}
			}

			var addToRoleResult = await userManager.AddToRoleAsync(user, "Admin");
			if (!addToRoleResult.Succeeded)
			{
				throw new Exception("Failed to assign admin role to the user.");
			}
		}

	}

}
