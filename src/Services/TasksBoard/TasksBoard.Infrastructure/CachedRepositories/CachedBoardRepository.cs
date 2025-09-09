using Common.Cache.Interfaces;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;

namespace TasksBoard.Infrastructure.CachedRepositories
{
    public class CachedBoardRepository(IBoardRepository decorated, ICacheRepository cache) : IBoardRepository
    {
        private readonly IBoardRepository _decorated = decorated;
        private readonly ICacheRepository _cache = cache;

        public void Add(Board entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountByUserIdAndQueryAsync(Guid userId, string query, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountPublicAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Delete(Board entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Board>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Board?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Board>> GetPaginatedAsync(int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Board>> GetPaginatedByUserIdAndQueryAsync(Guid userId, string query, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Board>> GetPaginatedByUserIdAsync(Guid userId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Board>> GetPaginatedPublicAsync(int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasAccessAsync(Guid boardId, Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Update(Board entity)
        {
            throw new NotImplementedException();
        }

        //public void Add(Board entity)
        //{
        //    string key = $"board_{entity.Id}";
        //    _cache.Create(key, entity);
        //    _decorated.Add(entity);
        //}

        //public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        //{
        //    return await _decorated.CountAsync(cancellationToken);
        //}

        //public async Task<int> CountByUserIdAndQueryAsync(Guid userId, string query, CancellationToken cancellationToken = default)
        //{
        //    return await _decorated.CountByUserIdAndQueryAsync(userId, query, cancellationToken);
        //}

        //public async Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        //{
        //    return await _decorated.CountByUserIdAsync(userId, cancellationToken);
        //}

        //public async Task<int> CountPublicAsync(CancellationToken cancellationToken = default)
        //{
        //    return await _decorated.CountPublicAsync(cancellationToken);
        //}

        //public void Delete(Board entity)
        //{
        //    string key = $"board_{entity.Id}";

        //    _cache.Remove(key);
        //    _decorated.Delete(entity);
        //}

        //public async Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default)
        //{
        //    return await _decorated.ExistAsync(id, cancellationToken);
        //}

        //public async Task<IEnumerable<Board>> GetAllAsync(CancellationToken cancellationToken = default)
        //{
        //    return await _decorated.GetAllAsync(cancellationToken);
        //}

        //public async Task<Board?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        //{
        //    string key = $"board_{id}";
        //    return await _cache.GetOrCreateAsync(key, () => _decorated.GetAsync(id, cancellationToken));
        //}

        //public async Task<IEnumerable<Board>> GetPaginatedAsync(int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        //{
        //    return await _decorated.GetPaginatedAsync(pageIndex, pageSize, cancellationToken);
        //}

        //public async Task<IEnumerable<Board>> GetPaginatedByUserIdAndQueryAsync(Guid userId, string query, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        //{
        //    return await _decorated.GetPaginatedByUserIdAndQueryAsync(userId, query, pageIndex, pageSize, cancellationToken);
        //}

        //public async Task<IEnumerable<Board>> GetPaginatedByUserIdAsync(Guid userId, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        //{
        //    return await  _decorated.GetPaginatedByUserIdAsync(userId, pageIndex, pageSize, cancellationToken);
        //}

        //public async Task<IEnumerable<Board>> GetPaginatedPublicAsync(int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        //{
        //    return await _decorated.GetPaginatedPublicAsync(pageIndex, pageSize, cancellationToken);
        //}

        //public async Task<bool> HasAccessAsync(Guid boardId, Guid userId, CancellationToken cancellationToken = default)
        //{
        //    return await _decorated.HasAccessAsync(boardId, userId, cancellationToken);
        //}

        //public void Update(Board entity)
        //{
        //    string key = $"board_{entity.Id}";
        //    _cache.Update(key, entity);
        //    _decorated.Update(entity);
        //}
    }
}
