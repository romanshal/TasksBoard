namespace Common.Blocks.ValueObjects
{
    public interface IGuidValueObject<TSelf> where TSelf : IGuidValueObject<TSelf>
    {
        static abstract TSelf Of(Guid value);
        static abstract TSelf Of(string value);
        static abstract TSelf New();
    }
}
