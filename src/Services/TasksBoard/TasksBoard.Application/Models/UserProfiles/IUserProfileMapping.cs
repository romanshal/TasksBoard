namespace TasksBoard.Application.Models.UserProfiles
{
    public interface IUserProfileMapping
    {
        IEnumerable<object> Items { get; }
        Func<object, Guid> IdSelector { get; }
        Action<object, string, string?> Setter { get; }

        void Deconstruct(out IEnumerable<object> items, out Func<object, Guid> idSelector, out Action<object, string, string?> setter);
    }
}
