using HEI.Support.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HEI.Support.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }
		//protected override void OnModelCreating(ModelBuilder modelBuilder)
		//{
		//	base.OnModelCreating(modelBuilder);

		//	// Customize the ASP.NET Core Identity model and override the defaults if needed.
		//	// For example, changing the table names:
		//	modelBuilder.Entity<ApplicationUser>(entity =>
		//	{
		//		entity.ToTable(name: "Users");
		//	});

		//	modelBuilder.Entity<IdentityRole>(entity =>
		//	{
		//		entity.ToTable(name: "Roles");
		//	});

		//	modelBuilder.Entity<IdentityUserRole<string>>(entity =>
		//	{
		//		entity.ToTable("UserRoles");
		//	});

		//	// Add more configurations here if needed
		//}
	}
}
