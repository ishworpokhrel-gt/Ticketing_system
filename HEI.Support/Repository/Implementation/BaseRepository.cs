using HEI.Support.Data.Entities;
using HEI.Support.Data;
using HEI.Support.Repository.Interface;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

namespace HEI.Support.Repository.Implementation
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        public readonly ApplicationDbContext _dbContext;
        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(T entity)
        {
            EntityEntry dbEntityEntry = _dbContext.Entry(entity);
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string Id, ApplicationUser user)
        {
            var entity = await _dbContext.Set<T>().FindAsync(Id);
            if (entity != null)
            {
                if (entity is BaseDbEntity baseEntity)
                {
                    baseEntity.IsDeleted = true;
                    baseEntity.UpdatedDate = DateTime.UtcNow;
                    baseEntity.UpdatedBy = user.Id;
                }

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<T> GetAsync(string id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

    }
}
