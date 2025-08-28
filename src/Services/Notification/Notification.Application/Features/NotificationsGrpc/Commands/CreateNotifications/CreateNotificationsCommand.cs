using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Notification.Application.Features.NotificationsGrpc.Commands.CreateNotifications
{
    public class CreateNotificationsCommand : IRequest<Result>
    {
        public required Guid EventId { get; set; }
        public required IEnumerable<Guid> AccountIds { get; set; }
        public required string EventType { get; set; }
        public required string Payload { get; set; }
    }
}
