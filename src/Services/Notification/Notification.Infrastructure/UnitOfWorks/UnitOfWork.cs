using Common.Blocks.UnitOfWorks;
using Notification.Domain.Entities;
using Notification.Domain.Interfaces.Repositories;
using Notification.Domain.Interfaces.UnitOfWorks;
using Notification.Domain.ValueObjects;
using Notification.Infrastructure.Data.Contexts;

namespace Notification.Infrastructure.UnitOfWorks
{
    public class UnitOfWork(
        IServiceProvider serviceProvider) : UnitOfWorkBase<NotificationDbContext>(serviceProvider), IUnitOfWork
    {
        public IApplicationEventRepository GetApplicationEventRepository()
        {
            return base.Repository<ApplicationEvent, ApplicationEventId, IApplicationEventRepository>();
        }
    }
}
