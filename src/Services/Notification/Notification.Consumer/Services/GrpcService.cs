using Common.gRPC.Protos;

namespace Notification.Consumer.Services
{
    internal class GrpcService(NotificationGrpc.NotificationGrpcClient grpcClient) : IGrpcService
    {
        public async Task Handle(CreateNotificationRequest request, CancellationToken cancellationToken = default)
        {
            await grpcClient.CreateNotificationAsync(request, cancellationToken: cancellationToken);
        }
    }
}
