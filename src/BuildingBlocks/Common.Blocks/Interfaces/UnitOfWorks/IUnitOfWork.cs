using Common.Blocks.Entities;
using Common.Blocks.Interfaces.Repositories;

namespace Common.Blocks.Interfaces.UnitOfWorks
{
    public interface IUnitOfWorkBase
    {
        IRepository<T> GetRepository<T>() where T : BaseEntity;
        TRepository GetCustomRepository<TRepository>() where TRepository : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
