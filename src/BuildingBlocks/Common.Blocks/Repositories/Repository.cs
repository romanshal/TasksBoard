using Common.Blocks.Entities;
using Common.Blocks.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Common.Blocks.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private bool _disposed;

        private readonly ILogger<Repository<T>> _logger;

        protected readonly DbContext Context;

        protected readonly DbSet<T> DbSet;

        public Repository(DbContext context, ILoggerFactory loggerFactory)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = loggerFactory.CreateLogger<Repository<T>>();
            DbSet = Context.Set<T>();
        }

        /// <summary>
        /// Get entity by id.
        /// </summary>
        /// <param name="id">Id of database entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id), $"Param 'id' is not valid.");
            }

            return await DbSet.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
        }

        /// <summary>
        /// Get all entities from database.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Get paginated entities from database.
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetPaginatedAsync(int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(e => e.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Get paginated entities from database by id.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetPaginatedByIdAsync(Guid id, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(entity => entity.Id == id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(e => e.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Add new entity to database.
        /// </summary>
        /// <param name="entity">Database entity.</param>
        /// <param name="needSaveChanges">Is need save changes.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Id of new entity</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task Add(T entity, bool needSaveChanges = false, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            DbSet.Add(entity);

            if (needSaveChanges)
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Update entity in database.
        /// </summary>
        /// <param name="entity">Database entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task Update(T entity, bool needSaveChanges = false, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            DbSet.Update(entity);

            if (needSaveChanges)
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Delete entity by id.
        /// </summary>
        /// <param name="entity">Database entity.</param>
        /// <param name="needSaveChanges">Is need save changes.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public virtual async Task Delete(T entity, bool needSaveChanges = false, CancellationToken cancellationToken = default)
        {
            DbSet.Remove(entity);

            if (needSaveChanges)
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Return count in sequence.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet.CountAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }

                _disposed = true;
            }
        }

        ~Repository()
        {
            Dispose(false);
        }
    }
}
