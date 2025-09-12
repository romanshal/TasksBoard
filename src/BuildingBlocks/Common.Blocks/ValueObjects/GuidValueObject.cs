using System.Linq.Expressions;
using System.Reflection;

namespace Common.Blocks.ValueObjects
{
    public abstract class GuidValueObject<TSelf> : ValueObject, IGuidValueObject<TSelf> where TSelf : GuidValueObject<TSelf>
    {
        public Guid Value { get; }

        protected GuidValueObject(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException($"{typeof(TSelf).Name} cannot be empty.", nameof(value));

            Value = value;
        }
        public static TSelf Of(Guid value) => ActivatorlessCtor<TSelf>.Create(value);

        public static TSelf New() => Of(Guid.NewGuid());

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value.ToString();

        private static class ActivatorlessCtor<T>
        {
            public static readonly Func<Guid, T> Create;

            static ActivatorlessCtor()
            {
                var ctor = typeof(T).GetConstructor(
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    binder: null,
                    [typeof(Guid)],
                    modifiers: null
                ) ?? throw new InvalidOperationException($"Type {typeof(T)} must have a private constructor with Guid parameter.");

                var param = Expression.Parameter(typeof(Guid), "value");
                var body = Expression.New(ctor, param);
                Create = Expression.Lambda<Func<Guid, T>>(body, param).Compile();
            }
        }
    }
}
