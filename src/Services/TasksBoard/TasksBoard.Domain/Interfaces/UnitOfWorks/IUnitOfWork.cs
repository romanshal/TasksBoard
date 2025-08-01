﻿using Common.Blocks.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Interfaces.Repositories;

namespace TasksBoard.Domain.Interfaces.UnitOfWorks
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
