using Common.Blocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces.Repositories;

namespace TaskBoard.Infrastructure.Repositories
{
    public class BoardNoticeRepository(DbContext context, ILoggerFactory loggerFactory) : Repository<BoardNotice>(context, loggerFactory), IBoardNoticeRepository
    {
    }
}
