using Common.Blocks.Models.Users;

namespace Common.Blocks.Interfaces.Services
{
    public interface IUserProfileService
    {
        Task<UserProfile?> ResolveAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyDictionary<Guid, UserProfile>> ResolveAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken = default);
    }
}
