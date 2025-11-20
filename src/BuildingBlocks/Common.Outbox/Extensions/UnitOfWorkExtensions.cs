using Common.Blocks.Interfaces.UnitOfWorks;
using Common.Outbox.Abstraction.Entities;
using Common.Outbox.Abstraction.Interfaces.Repositories;
using Common.Outbox.Abstraction.ValueObjects;

namespace Common.Outbox.Extensions
{
    public static class UnitOfWorkExtensions
    {
        public static IOutboxEventRepository GetOutboxEventRepository(this IUnitOfWorkBase unitOfWork)
        {
            //return unitOfWork.GetCustomRepository<OutboxEventRepository>();
            return unitOfWork.GetRepository<OutboxEvent, OutboxId, IOutboxEventRepository>();
        }
    }
}
