using Common.Outbox.Abstraction.ValueObjects;
using Dapper;
using System.Data;

namespace Common.Outbox.Handlers
{
    internal sealed class OutboxIdTypeHandler : SqlMapper.TypeHandler<OutboxId>
    {
        public override void SetValue(IDbDataParameter parameter, OutboxId value)
        => parameter.Value = value.Value;

        public override OutboxId Parse(object value)
            => OutboxId.Of((Guid)value);
    }
}
