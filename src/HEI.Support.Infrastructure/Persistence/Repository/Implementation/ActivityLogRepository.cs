using HEI.Support.Domain.Entities;
using HEI.Support.Infrastructure.Persistence.Repository.Interface;

namespace HEI.Support.Infrastructure.Persistence.Repository.Implementation
{
    public class ActivityLogRepository : BaseRepository<ActivityLog>, IBaseRepository<ActivityLog>, IActivityLogRepository
    {
        public ActivityLogRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
