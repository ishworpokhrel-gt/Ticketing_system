using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEI.Support.Infrastructure.Persistence.Repository.Interface
{
    public interface IUnitOfWorkRepository
    {
        void BeginTransaction();
        void Commit();
        void Dispose();
        void Rollback();
    }
}
