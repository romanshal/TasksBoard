using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notification.Application.Dtos;
using Notification.Application.Handlers;
using Notification.Application.Interfaces.HubServices;
using Notification.Domain.Interfaces.UnitOfWorks;

namespace Notification.Application.BackgroundServices
{
    public class NotificationBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<NotificationBackgroundService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            using IServiceScope scope = serviceScopeFactory.CreateScope();

            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
            var hubNotificationService = scope.ServiceProvider.GetRequiredService<IHubNotificationService>();
            var profileHandler = scope.ServiceProvider.GetRequiredService<UserProfileHandler>();

            var lastDate = DateTime.Now;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var applicationEvents = await unitOfWork.GetApplicationEventRepository().GetNewByCreatedDateAsync(lastDate, stoppingToken);
                    if (!applicationEvents.Any())
                    {
                        continue;
                    }

                    foreach (var applicationEvent in applicationEvents)
                    {
                        var notification = mapper.Map<NotificationDto>(applicationEvent);

                        await profileHandler.Handle(notification, stoppingToken);

                        await hubNotificationService.NotifyUserAsync(applicationEvent.AccountId.ToString(), notification, stoppingToken);
                    }

                    lastDate = applicationEvents.Last().CreatedAt;
                }
                catch (Exception ex)
                {
                    logger.LogError("Notification background service error {error}", ex.Message);
                }
                finally
                {
                    await Task.Delay(2500, stoppingToken);
                }
            }
        }
    }
}
