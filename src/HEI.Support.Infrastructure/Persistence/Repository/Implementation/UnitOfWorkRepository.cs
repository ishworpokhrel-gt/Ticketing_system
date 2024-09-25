using HEI.Support.Infrastructure.Persistence.Repository.Interface;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HEI.Support.Infrastructure.Persistence.Repository.Implementation.UnitOfWorkRepository;

namespace HEI.Support.Infrastructure.Persistence.Repository.Implementation
{
        public class UnitOfWorkRepository : IUnitOfWorkRepository
        {
            private readonly ApplicationDbContext _dbContext;
            private IDbContextTransaction _transaction;

            public UnitOfWorkRepository(ApplicationDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public void BeginTransaction()
            {
                _transaction = _dbContext.Database.BeginTransaction();
            }

            public void Commit()
            {
                _transaction.Commit();
            }

            public void Rollback()
            {
                _transaction.Rollback();
            }

            public void Dispose()
            {
                _transaction.Dispose();
            }
        }
    
}
