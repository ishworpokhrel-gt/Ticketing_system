using HEI.Support.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HEI.Support.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }
		public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<AttachmentFile> AttachmentFiles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

	}
}
