using TasksBoard.Domain.Models;

namespace TasksBoard.Domain.Interfaces.Services
{
    public interface IUserProfileService
    {
        Task<IReadOnlyDictionary<Guid, UserProfile>> ResolveAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken = default);
    }
}
