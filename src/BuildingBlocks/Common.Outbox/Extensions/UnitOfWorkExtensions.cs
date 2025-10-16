using Common.Blocks.Interfaces.UnitOfWorks;
using Common.Outbox.Abstraction.Interfaces.Repositories;
using Common.Outbox.Repositories;

namespace Common.Outbox.Extensions
{
    public static class UnitOfWorkExtensions
    {
        public static IOutboxEventRepository GetOutboxEventRepository(this IUnitOfWorkBase unitOfWork)
        {
            return unitOfWork.GetCustomRepository<OutboxEventRepository>();
        }
    }
}
