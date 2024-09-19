using HEI.Support.Data.Entities;

namespace HEI.Support.Repository.Interface
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
