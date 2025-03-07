using Common.Blocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskBoard.Infrastructure.Entities;
using TaskBoard.Infrastructure.Interfaces.Repositories;

namespace TaskBoard.Infrastructure.Repositories
{
    public class NoticeRepository(DbContext context, ILoggerFactory loggerFactory) : Repository<BoardNotice>(context, loggerFactory), INoticeRepository
    {
    }
}
