using Common.gRPC.Protos;

namespace Notification.Consumer.Services
{
    public interface IGrpcService
    {
        Task Handle(CreateNotificationRequest request, CancellationToken cancellationToken = default);
    }
}
