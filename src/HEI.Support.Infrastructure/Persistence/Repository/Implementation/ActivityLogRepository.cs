using HEI.Support.Domain.Entities;
using HEI.Support.Infrastructure.Persistence.Repository.Interface;

namespace HEI.Support.Infrastructure.Persistence.Repository.Implementation
{
    public class ActivityLogRepository : BaseRepository<ActivityLog>, IBaseRepository<ActivityLog>, IActivityLogRepository
    {
        private readonly ApplicationDbContext _context;
        public ActivityLogRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _context = applicationDbContext;
        }
    }
}
