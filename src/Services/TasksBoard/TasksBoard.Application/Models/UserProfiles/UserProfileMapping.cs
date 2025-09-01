namespace TasksBoard.Application.Models.UserProfiles
{
    internal record UserProfileMapping(
            IEnumerable<object> Items,
            Func<object, Guid> IdSelector,
            Action<object, string, string?> Setter) : IUserProfileMapping
    {
        public void Deconstruct(out IEnumerable<object> items, out Func<object, Guid> idSelector, out Action<object, string, string?> setter)
        {
            items = Items;
            idSelector = IdSelector;
            setter = Setter;
        }
    }
}
