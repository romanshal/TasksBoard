using TasksBoard.Application.Models.UserProfiles;

namespace TasksBoard.Application.Handlers
{
    public interface IUserProfileHandler
    {
        Task Handle<T>(
            T value,
            Func<T, Guid> accountIdSelector,
            Action<T, string, string?> accountNameSetter,
            CancellationToken cancellationToken = default);

        Task Handle<T>(
            IEnumerable<T> values,
            Func<T, Guid> accountIdSelector,
            Action<T, string, string?> accountProfileSetter,
            CancellationToken cancellationToken = default);

        Task HandleMany(
           IEnumerable<IUserProfileMapping> mappings,
           CancellationToken cancellationToken = default);
    }
}
