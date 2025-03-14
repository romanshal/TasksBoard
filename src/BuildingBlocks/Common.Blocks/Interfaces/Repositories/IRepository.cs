using Common.Blocks.Entities;

namespace Common.Blocks.Interfaces.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Get entity by id.
        /// </summary>
        /// <param name="id">Id of database entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all entities from database.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get paginated entities from database.
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetPaginatedAsync(int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default);

        /// <summary>
        /// Add new entity to database.
        /// </summary>
        /// <param name="entity">Database entity.</param>
        /// <param name="needSaveChanges">Is need save changes.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Id of new entity</returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task Add(T entity, bool needSaveChanges = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update entity in database.
        /// </summary>
        /// <param name="entity">Database entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task Update(T entity, bool needSaveChanges = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete entity by id.
        /// </summary>
        /// <param name="entity">Database entity.</param>
        /// <param name="needSaveChanges">Is need save changes.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task Delete(T entity, bool needSaveChanges = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Return count in sequence.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);
    }
}
