namespace Common.Blocks.ValueObjects
{
    public sealed class AccountId : GuidValueObject<AccountId>
    {
        private AccountId(Guid value) : base(value) { }
    }
}
