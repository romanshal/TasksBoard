using Common.Cache.Interfaces;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.ValueObjects;
using TasksBoard.Infrastructure.CacheBuffers;
using TasksBoard.Infrastructure.Factories;

namespace TasksBoard.Infrastructure.CachedRepositories
{
    internal class CachedBoardRepository(
        IBoardRepository decorated,
        ICacheRepository cache,
        ICacheKeyFactory<Board, BoardId> keyFactory,
        IAsyncCacheTransactionBuffer cacheBuffer) : IBoardRepository
    {
        private readonly IBoardRepository _decorated = decorated;
        private readonly ICacheRepository _cache = cache;
        private readonly ICacheKeyFactory<Board, BoardId> _keyFactory = keyFactory;
        private readonly IAsyncCacheTransactionBuffer _cacheBuffer = cacheBuffer;

        public void Add(Board entity)
        {
            string key = _keyFactory.Key(entity.Id);

            _decorated.Add(entity);

            _cacheBuffer.AddPending(() => _cache.CreateAsync(key, entity));
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _decorated.CountAsync(cancellationToken);
        }

        public async Task<int> CountByUserIdAndQueryAsync(Guid userId, string query, CancellationToken cancellationToken = default)
        {
            return await _decorated.CountByUserIdAndQueryAsync(userId, query, cancellationToken);
        }

        public async Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _decorated.CountByUserIdAsync(userId, cancellationToken);
        }

        public async Task<int> CountPublicAsync(CancellationToken cancellationToken = default)
        {
            return await _decorated.CountPublicAsync(cancellationToken);
        }

        public void Delete(Board entity)
        {
            string key = _keyFactory.Key(entity.Id);

            _decorated.Delete(entity);

            _cacheBuffer.AddPending(() => _cache.RemoveAsync(key));
        }

        public async Task<bool> ExistAsync(BoardId id, CancellationToken cancellationToken = default)
        {
            return await _decorated.ExistAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<Board>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _decorated.GetAllAsync(cancellationToken);
        }

        public async Task<Board?> GetAsync(BoardId id, CancellationToken cancellationToken = default)
        {
            string key = _keyFactory.Key(id);
            return await _cache.GetOrCreateAsync(key, () => _decorated.GetAsync(id, cancellationToken));
        }

        public async Task<IEnumerable<Board>> GetPaginatedAsync(int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await _decorated.GetPaginatedAsync(pageIndex, pageSize, cancellationToken);
        }

        public async Task<IEnumerable<Board>> GetPaginatedByUserIdAndQueryAsync(Guid userId, string query, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await _decorated.GetPaginatedByUserIdAndQueryAsync(userId, query, pageIndex, pageSize, cancellationToken);
        }

        public async Task<IEnumerable<Board>> GetPaginatedByUserIdAsync(Guid userId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await _decorated.GetPaginatedByUserIdAsync(userId, pageIndex, pageSize, cancellationToken);
        }

        public async Task<IEnumerable<Board>> GetPaginatedPublicAsync(int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await _decorated.GetPaginatedPublicAsync(pageIndex, pageSize, cancellationToken);
        }

        public async Task<bool> HasAccessAsync(BoardId boardId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _decorated.HasAccessAsync(boardId, userId, cancellationToken);
        }

        public void Update(Board entity)
        {
            string key = _keyFactory.Key(entity.Id);

            _decorated.Update(entity);

            _cacheBuffer.AddPending(() => _cache.CreateAsync(key, entity));
        }
    }
}
