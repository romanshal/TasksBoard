using Common.Blocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Repositories;

namespace TasksBoard.Infrastructure.Repositories
{
    public class BoardNoticeRepository(DbContext context, ILoggerFactory loggerFactory) : Repository<BoardNotice>(context, loggerFactory), IBoardNoticeRepository
    {
    }
}
