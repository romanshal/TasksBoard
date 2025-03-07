using Common.Blocks.Entities;

namespace Common.Blocks.Interfaces.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
