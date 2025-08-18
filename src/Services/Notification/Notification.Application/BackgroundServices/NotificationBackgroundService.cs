using AutoMapper;
using Common.Blocks.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notification.Application.Dtos;
using Notification.Application.Interfaces.HubServices;
using Notification.Domain.Constants;
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
            var hubNotificationService = scope.ServiceProvider.GetRequiredService<IHubNotificationService>();
            var profileService = scope.ServiceProvider.GetRequiredService<IUserProfileService>();

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

                        var accountIdExist = notification.Payload.TryGetValue(NotificationLinkTypes.AccountId, out var accountId);
                        var sourceAccountIdExist = notification.Payload.TryGetValue(NotificationLinkTypes.SourceAccountId, out var sourceAccountId);
                        var ids = new List<Guid>();

                        if (accountIdExist && !string.IsNullOrWhiteSpace(accountId))
                        {
                            ids.Add(Guid.Parse(accountId));
                        }

                        if(sourceAccountIdExist && !string.IsNullOrWhiteSpace(sourceAccountId))
                        {
                            ids.Add(Guid.Parse(sourceAccountId));
                        }

                        if (ids.Count > 0)
                        {
                            var userProfiles = await profileService.ResolveAsync(ids, stoppingToken);
                            if (userProfiles.Count > 0)
                            {
                                if (accountIdExist && !string.IsNullOrWhiteSpace(accountId))
                                {
                                    var isExist = userProfiles.TryGetValue(Guid.Parse(accountId), out var profile);
                                    if (isExist && profile is not null)
                                    {
                                        notification.Payload.Add(NotificationLinkTypes.AccountName, profile.Username);
                                    }
                                }

                                if (sourceAccountIdExist && !string.IsNullOrWhiteSpace(sourceAccountId))
                                {
                                    var isExist = userProfiles.TryGetValue(Guid.Parse(sourceAccountId), out var profile);
                                    if (isExist && profile is not null)
                                    {
                                        notification.Payload.Add(NotificationLinkTypes.SourceAccountName, profile.Username);
                                    }
                                }
                            }
                        }

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
