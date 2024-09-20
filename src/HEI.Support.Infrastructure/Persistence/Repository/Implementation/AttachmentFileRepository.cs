using HEI.Support.Domain.Entities;
using HEI.Support.Infrastructure.Persistence.Repository.Interface;

namespace HEI.Support.Infrastructure.Persistence.Repository.Implementation
{
    public class AttachmentFileRepository : BaseRepository<AttachmentFile>, IBaseRepository<AttachmentFile>, IAttachmentFileRepository
    {
        public AttachmentFileRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
