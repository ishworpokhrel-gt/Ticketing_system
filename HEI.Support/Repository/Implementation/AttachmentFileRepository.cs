using HEI.Support.Data;
using HEI.Support.Data.Entities;
using HEI.Support.Repository.Interface;

namespace HEI.Support.Repository.Implementation
{
    public class AttachmentFileRepository : BaseRepository<AttachmentFile>, IBaseRepository<AttachmentFile>, IAttachmentFileRepository
    {
        public AttachmentFileRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
