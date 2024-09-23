using HEI.Support.Domain.Entities;

namespace HEI.Support.Infrastructure.Persistence.Repository.Interface
{
    public interface IBaseRepository<T> where T : class
    {
        Task AddAsync(T entity);

        Task<T> GetAsync(Guid id);

        Task UpdateAsync(T entity);

        Task DeleteAsync(string id, ApplicationUser user);

        Task<List<T>> GetAllAsync();
        Task AddMultipleEntity<TEntity>(IEnumerable<TEntity> entityList) where TEntity : class;
    }
}
