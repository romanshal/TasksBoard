using Common.Blocks.Repositories;
using Common.Blocks.UnitOfWorks;
using Microsoft.Extensions.Logging;
using Notification.Domain.Entities;
using Notification.Domain.Interfaces.Repositories;
using Notification.Domain.Interfaces.UnitOfWorks;
using Notification.Infrastructure.Data.Contexts;
using Notification.Infrastructure.Repositories;

namespace Notification.Infrastructure.UnitOfWorks
{
    public class UnitOfWork(
        NotificationDbContext context,
        ILoggerFactory loggerFactory) : UnitOfWorkBase(context, loggerFactory), IUnitOfWork
    {
        private readonly NotificationDbContext _context = context;
        private readonly ILoggerFactory _loggerFactory = loggerFactory;

        public IApplicationEventRepository GetApplicationEventRepository()
        {
            var type = typeof(ApplicationEvent);

            if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<ApplicationEvent>))
            {
                var repositoryInstance = new ApplicationEventRepository(_context, _loggerFactory);

                value = repositoryInstance;

                _repositories[type] = repositoryInstance;
            }

            return (IApplicationEventRepository)value;
        }
    }
}
