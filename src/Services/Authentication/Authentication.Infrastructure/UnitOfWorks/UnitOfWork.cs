using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Repositories;
using Authentication.Domain.Interfaces.UnitOfWorks;
using Authentication.Infrastructure.Data.Contexts;
using Authentication.Infrastructure.Repositories;
using Common.Blocks.Repositories;
using Common.Blocks.UnitOfWorks;
using Microsoft.Extensions.Logging;

namespace Authentication.Infrastructure.UnitOfWorks
{
    public class UnitOfWork(
        AuthenticationDbContext context,
        ILoggerFactory loggerFactory) : UnitOfWorkBase(context, loggerFactory), IUnitOfWork
    {
        private readonly AuthenticationDbContext _context = context;
        private readonly ILoggerFactory _loggerFactory = loggerFactory;

        public IApplicationUserImageRepository GetApplicationUserImageRepository()
        {
            var type = typeof(ApplicationUserImage);

            if (!_repositories.TryGetValue(type, out object? value) || value.GetType() == typeof(Repository<ApplicationUserImage>))
            {
                var repositoryInstance = new ApplicationUserImageRepository(_context, _loggerFactory);

                value = repositoryInstance;

                _repositories[type] = repositoryInstance;
            }

            return (IApplicationUserImageRepository)value;
        }
    }
}
