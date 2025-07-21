using Common.Blocks.Interfaces.UnitOfWorks;
using TasksBoard.Application.Interfaces.Repositories;

namespace TasksBoard.Application.Interfaces.UnitOfWorks
{
    public interface IUnitOfWork : IUnitOfWorkBase
    {
        IBoardNoticeRepository GetBoardNoticeRepository();
        IBoardRepository GetBoardRepository();
        IBoardMemberRepository GetBoardMemberRepository();
        IBoardAccessRequestRepository GetBoardAccessRequestRepository();
        IBoardInviteRequestRepository GetBoardInviteRequestRepository();
    }
}
