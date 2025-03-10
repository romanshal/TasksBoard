using Common.Blocks.Entities;

namespace Common.Blocks.Interfaces.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetPaginatedAsync(int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task Add(T entity, bool needSaveChanges = false, CancellationToken cancellationToken = default);
        Task Update(T entity, bool needSaveChanges = false, CancellationToken cancellationToken = default);
        Task Delete(T entity, bool needSaveChanges = false, CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
    }
}
