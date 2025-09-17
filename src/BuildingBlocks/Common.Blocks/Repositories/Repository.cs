using Common.Blocks.Entities;
using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Common.Blocks.Repositories
{
    public class Repository<T, TId> : IRepository<T, TId>
        where T : class, IEntity<TId>
        where TId : ValueObject
    {
        private bool _disposed;

        private readonly ILogger<Repository<T, TId>> _logger;

        protected readonly DbContext Context;

        protected readonly DbSet<T> DbSet;

        public Repository(DbContext context, ILoggerFactory loggerFactory)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = loggerFactory.CreateLogger<Repository<T, TId>>();
            DbSet = Context.Set<T>();
        }

        /// <summary>
        /// Get entity by id.
        /// </summary>
        /// <param name="id">Id of database entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<T?> GetAsync(TId id, CancellationToken cancellationToken = default)
        {
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
        /// Add new entity to database.
        /// </summary>
        /// <param name="entity">Database entity.</param>
        /// <returns>Id of new entity</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void Add(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            DbSet.Add(entity);
        }

        /// <summary>
        /// Update entity in database.
        /// </summary>
        /// <param name="entity">Database entity.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void Update(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            DbSet.Update(entity);
        }

        /// <summary>
        /// Delete entity by id.
        /// </summary>
        /// <param name="entity">Database entity.</param>
        /// <returns></returns>
        public virtual void Delete(T entity)
        {
            DbSet.Remove(entity);
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

        /// <summary>
        /// If any element exist in database.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public virtual async Task<bool> ExistAsync(TId id, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .AnyAsync(entity => entity.Id == id, cancellationToken);
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
