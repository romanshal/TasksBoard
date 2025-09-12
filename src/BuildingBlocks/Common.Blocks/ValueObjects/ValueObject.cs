namespace Common.Blocks.ValueObjects
{
    public abstract class ValueObject
    {
        protected abstract IEnumerable<object?> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != GetType()) return false;
            return GetEqualityComponents().SequenceEqual(((ValueObject)obj).GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Aggregate(0, (hash, obj) => HashCode.Combine(hash, obj?.GetHashCode() ?? 0));
        }

        public static bool operator ==(ValueObject? a, ValueObject? b) => Equals(a, b);
        public static bool operator !=(ValueObject? a, ValueObject? b) => !Equals(a, b);
    }
}
