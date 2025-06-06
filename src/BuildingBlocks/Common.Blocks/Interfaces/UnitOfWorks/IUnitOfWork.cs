using Common.Blocks.Entities;
using Common.Blocks.Interfaces.Repositories;

namespace Common.Blocks.Interfaces.UnitOfWorks
{
    public interface IUnitOfWorkBase
    {
        IRepository<T> GetRepository<T>() where T : BaseEntity;
        IOutboxEventRepository GetOutboxEventRepository();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
