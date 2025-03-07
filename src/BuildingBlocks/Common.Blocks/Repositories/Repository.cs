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
        /// <param name="id">Id of database entity</param>
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
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet.ToListAsync(cancellationToken);
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
        /// <param name="id">Id of database entity.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void Delete(T entity)
        {
            DbSet.Remove(entity);
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
