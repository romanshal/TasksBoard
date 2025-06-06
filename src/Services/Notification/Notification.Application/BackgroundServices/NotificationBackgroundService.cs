using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notification.Application.Dtos;
using Notification.Application.Hubs;
using Notification.Domain.Interfaces.UnitOfWorks;

namespace Notification.Application.BackgroundServices
{
    public class NotificationBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<NotificationBackgroundService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();

            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

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
                        await hubContext.Clients.User(applicationEvent.AccountId.ToString()).SendAsync("ReceiveNotification", notification, stoppingToken);
                    }

                    lastDate = applicationEvents.Last().CreatedAt;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    await Task.Delay(2500, stoppingToken);
                }
            }
        }
    }
}
