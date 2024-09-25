using HEI.Support.Domain.Entities;

namespace HEI.Support.Infrastructure.Persistence.Repository.Interface
{
    public interface IActivityLogRepository : IBaseRepository<ActivityLog>
    {
        Task<ActivityLog> GetActivityLogByTicketIdAndStatus(Guid ticketId, int status);
    }
}
