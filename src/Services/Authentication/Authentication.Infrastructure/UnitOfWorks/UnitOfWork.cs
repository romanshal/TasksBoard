using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Repositories;
using Authentication.Domain.Interfaces.UnitOfWorks;
using Authentication.Domain.ValueObjects;
using Authentication.Infrastructure.Data.Contexts;
using Common.Blocks.Repositories;
using Common.Blocks.UnitOfWorks;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.Infrastructure.UnitOfWorks
{
    public class UnitOfWork(
        IServiceProvider serviceProvider) : UnitOfWorkBase<AuthenticationDbContext>(serviceProvider), IUnitOfWork
    {
        public IApplicationUserImageRepository GetApplicationUserImageRepository()
        {
            var type = typeof(ApplicationUserImage);

            if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<ApplicationUserImage, ImageId>))
            {
                var repositoryInstance = _scope.ServiceProvider.GetRequiredService<IApplicationUserImageRepository>();

                value = repositoryInstance;

                _repositories[type] = repositoryInstance;
            }

            return (IApplicationUserImageRepository)value;
        }
    }
}
