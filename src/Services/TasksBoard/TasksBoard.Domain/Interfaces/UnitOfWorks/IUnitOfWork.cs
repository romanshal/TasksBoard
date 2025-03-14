using Common.Blocks.Entities;
using Common.Blocks.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.Repositories;

namespace TasksBoard.Domain.Interfaces.UnitOfWorks
{
    public interface IUnitOfWork
    {
        IRepository<T> GetRepository<T>() where T : BaseEntity;
        IBoardNoticeRepository GetBoardNoticeRepository();
        IBoardRepository GetBoardRepository();
        IBoardMemberRepository GetBoardMemberRepository();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
