using Common.Blocks.UnitOfWorks;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;
using TasksBoard.Infrastructure.Data.Contexts;

namespace TasksBoard.Infrastructure.UnitOfWorks
{
    internal class UnitOfWork(
        IServiceProvider serviceProvider) : UnitOfWorkBase<TasksBoardDbContext>(serviceProvider), IUnitOfWork
    {
        public IBoardNoticeRepository GetBoardNoticeRepository()
        {
            return base.GetRepository<BoardNotice, BoardNoticeId, IBoardNoticeRepository>();
        }

        public IBoardRepository GetBoardRepository()
        {
            return base.GetRepository<Board, BoardId, IBoardRepository>();
        }

        public IBoardMemberRepository GetBoardMemberRepository()
        {
            return base.GetRepository<BoardMember, BoardMemberId, IBoardMemberRepository>();
        }

        public IBoardAccessRequestRepository GetBoardAccessRequestRepository()
        {
            return base.GetRepository<BoardAccessRequest, BoardAccessId, IBoardAccessRequestRepository>();
        }

        public IBoardInviteRequestRepository GetBoardInviteRequestRepository()
        {
            return base.GetRepository<BoardInviteRequest, BoardInviteId, IBoardInviteRequestRepository>();
        }
    }
}
