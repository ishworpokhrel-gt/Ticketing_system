using HEI.Support.Data;
using HEI.Support.Data.Entities;
using HEI.Support.Repository.Interface;

namespace HEI.Support.Repository.Implementation
{
    public class CommentRepository :BaseRepository<Comment>, IBaseRepository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
