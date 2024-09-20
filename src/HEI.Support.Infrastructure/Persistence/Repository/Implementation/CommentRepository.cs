using HEI.Support.Domain.Entities;
using HEI.Support.Infrastructure.Persistence.Repository.Interface;

namespace HEI.Support.Infrastructure.Persistence.Repository.Implementation
{
    public class CommentRepository :BaseRepository<Comment>, IBaseRepository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
