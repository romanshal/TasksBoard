using Common.Blocks.Repositories;
using Common.Blocks.UnitOfWorks;
using Microsoft.Extensions.DependencyInjection;
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
            var type = typeof(ApplicationEvent);

            if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<ApplicationEvent, ApplicationEventId>))
            {
                var repositoryInstance = _scope.ServiceProvider.GetRequiredService<IApplicationEventRepository>();

                value = repositoryInstance;

                _repositories[type] = repositoryInstance;
            }

            return (IApplicationEventRepository)value;
        }
    }
}
