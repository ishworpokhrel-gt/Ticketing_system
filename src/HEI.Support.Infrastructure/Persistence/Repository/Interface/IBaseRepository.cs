using HEI.Support.Domain.Entities;

namespace HEI.Support.Infrastructure.Persistence.Repository.Interface
{
    public interface IBaseRepository<T> where T : class
    {
        Task AddAsync(T entity);

        Task<T> GetAsync(string id);

        Task UpdateAsync(T entity);

        Task DeleteAsync(string id, ApplicationUser user);

        Task<List<T>> GetAllAsync();
    }
}
