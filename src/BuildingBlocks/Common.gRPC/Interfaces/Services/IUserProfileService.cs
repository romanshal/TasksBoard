using Common.gRPC.Models;

namespace Common.gRPC.Interfaces.Services
{
    public interface IUserProfileService
    {
        Task<UserProfile?> ResolveAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyDictionary<Guid, UserProfile>> ResolveAsync(
            HashSet<Guid> ids,
            CancellationToken cancellationToken = default);
    }
}
