using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace Notification.Application.Features.NotificationsGrpc.Commands.CreateNotifications
{
    public class CreateNotificationsCommand : ICommand<Result>
    {
        public required Guid EventId { get; set; }
        public required IEnumerable<Guid> AccountIds { get; set; }
        public required string EventType { get; set; }
        public required string Payload { get; set; }
    }
}
