using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Repositories;
using Authentication.Infrastructure.Data.Contexts;
using Common.Blocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Infrastructure.Repositories
{
    public class ApplicationUserImageRepository(
        AuthenticationDbContext context,
        ILoggerFactory loggerFactory) : Repository<ApplicationUserImage>(context, loggerFactory), IApplicationUserImageRepository
    {
        public async Task<ApplicationUserImage?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(image => image.UserId == userId, cancellationToken);
        }
    }
}
