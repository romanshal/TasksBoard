using Common.Blocks.Entities;
using Common.Blocks.Interfaces.Repositories;
using Common.Blocks.ValueObjects;

namespace Common.Blocks.Interfaces.UnitOfWorks
{
    public interface IUnitOfWorkBase
    {
        IRepository<T, TId> GetRepository<T, TId>() where T : class, IEntity<TId> where TId : ValueObject;
        TRepository GetCustomRepository<TRepository>() where TRepository : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
