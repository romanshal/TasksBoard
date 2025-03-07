using Common.Blocks.Entities;
using Common.Blocks.Interfaces.Repositories;

namespace TasksBoard.Domain.Interfaces.UnitOfWorks
{
    public interface IUnitOfWork
    {
        IRepository<T> GetRepository<T>() where T : BaseEntity;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
