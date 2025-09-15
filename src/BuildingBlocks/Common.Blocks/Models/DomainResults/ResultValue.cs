using Newtonsoft.Json;

namespace Common.Blocks.Models.DomainResults
{
    public class Result<T> : Result
    {
        private readonly T? _value;

        [JsonConstructor]
        protected internal Result(T? value, bool isSuccess, Error error) : base(isSuccess, error) =>
            _value = value;

        //TODO: return this and change logic
        //[NotNull]
        //public T Value => _value! ?? throw new InvalidOperationException("Result has no value.");

        public T? Value => _value;

        public static implicit operator Result<T>(T? value) => Create(value);
    }
}
