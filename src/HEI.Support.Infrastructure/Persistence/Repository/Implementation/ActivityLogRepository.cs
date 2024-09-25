using HEI.Support.Domain.Entities;
using HEI.Support.Infrastructure.Persistence.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace HEI.Support.Infrastructure.Persistence.Repository.Implementation
{
    public class ActivityLogRepository : BaseRepository<ActivityLog>, IBaseRepository<ActivityLog>, IActivityLogRepository
    {
        private readonly ApplicationDbContext _context;
        public ActivityLogRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public async Task<ActivityLog> GetActivityLogByTicketIdAndStatus(Guid ticketId, int status)
        {
            var activity = await _context.ActivityLogs.Where(a => a.TicketId == ticketId && a.Status == status).FirstOrDefaultAsync();
            return activity;
        }
    }
}
