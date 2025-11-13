using Common.gRPC.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Notification.Application.Features.NotificationsGrpc.Commands.CreateNotifications;

namespace Notification.API.Controllers
{
    public class NotificationGrpcController(IMediator mediator) : NotificationGrpc.NotificationGrpcBase
    {
        public override async Task<Empty> CreateNotification(CreateNotificationRequest request, ServerCallContext context)
        {
            var result = await mediator.Send(new CreateNotificationsCommand
            {
                EventId = Guid.Parse(request.EventId),
                AccountIds = request.AccountIds.Select(Guid.Parse),
                EventType = request.EventType,
                Payload = request.Payload
            });

            if (result.IsFailure)
            {
                //TODO: add response
            }

            return new Empty();
        }
    }
}
