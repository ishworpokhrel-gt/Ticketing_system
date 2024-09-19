using HEI.Support.Data;
using HEI.Support.Data.Entities;
using HEI.Support.Repository.Interface;

namespace HEI.Support.Repository.Implementation
{
    public class ActivityLogRepository : BaseRepository<ActivityLog>, IBaseRepository<ActivityLog>, IActivityLogRepository
    {
        public ActivityLogRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
