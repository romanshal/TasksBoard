using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Repositories;
using Authentication.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Infrastructure.Repositories
{
    public class ApplicationUserSessionRepository(AuthenticationDbContext context) : IApplicationUserSessionRepository
    {
        private readonly AuthenticationDbContext _context = context;

        public async Task<IEnumerable<ApplicationUserSession>> GetByUserIdAsync(
            Guid userId, 
            CancellationToken cancellationToken = default)
        {
            return await _context.Sessions
                .Where(t => t.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<ApplicationUserSession?> GetByUserIdAndDeviceId(
            Guid userId,
            string deviceId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Sessions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.DeviceId == deviceId && !s.RevokedAtUtc.HasValue, cancellationToken);
        }

        public void Add(ApplicationUserSession token)
        {
            ArgumentNullException.ThrowIfNull(token);

            _context.Sessions.Add(token);
        }

        public void Update(ApplicationUserSession token)
        {
            ArgumentNullException.ThrowIfNull(token);

            _context.Sessions.Update(token);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
