using AutoMapper;
using EventBus.Messages.Events;
using Notification.Application.Constants;
using Notification.Application.Dtos;
using Notification.Domain.Entities;
using System.Text.Json;

namespace Notification.Application.Mappings
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<ApplicationEvent, NotificationDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => GetNotificationType(src.EventType)))
                .ForMember(dest => dest.Payload, opt => opt.MapFrom(src => GetNotificationPayload(src.EventType, src.Payload)));
        }

        private static string GetNotificationType(string eventType)
        {
            return eventType switch
            {
                nameof(NewNoticeEvent) => Enum.GetName(NotificationTypes.NewNotice)!,
                nameof(UpdateNoticeStatusEvent) => Enum.GetName(NotificationTypes.UpdateNoticeComplete)!,
                nameof(NewBoardMemberEvent) => Enum.GetName(NotificationTypes.NewBoardMember)!,
                nameof(RemoveBoardMemberEvent) => Enum.GetName(NotificationTypes.RemoveBoardMember)!,
                nameof(ResolveAccessRequestEvent) => Enum.GetName(NotificationTypes.ResolveAccessRequest)!,
                nameof(NewBoardInviteRequestEvent) => Enum.GetName(NotificationTypes.NewBoardInvite)!,
                nameof(NewBoardAccessRequestEvent) => Enum.GetName(NotificationTypes.NewBoardAccessRequest)!,
                nameof(NewBoardMemberPermissionsEvent) => Enum.GetName(NotificationTypes.NewMemberPermissions)!,
                nameof(UpdateNoticeEvent) => Enum.GetName(NotificationTypes.UpdateNotice)!,
                _ => Enum.GetName(NotificationTypes.Default)!,
            };
        }

        private static Dictionary<NotificationLinkTypes, string> GetNotificationPayload(string eventType, string eventPayload)
        {
            var notificationPayload = new Dictionary<NotificationLinkTypes, string>();

            if (eventType == nameof(NewNoticeEvent))
            {
                var payload = JsonSerializer.Deserialize<NewNoticeEvent>(eventPayload)!;

                notificationPayload.Add(NotificationLinkTypes.BoardId, payload.BoardId.ToString());
                notificationPayload.Add(NotificationLinkTypes.BoardName, payload.BoardName);
                notificationPayload.Add(NotificationLinkTypes.NoticeId, payload.NoticeId.ToString());
                notificationPayload.Add(NotificationLinkTypes.NoticeDefinition, payload.NoticeDefinition);
                notificationPayload.Add(NotificationLinkTypes.AccountId, payload.AccountId.ToString());
                notificationPayload.Add(NotificationLinkTypes.AccountName, payload.AccountName);
            }
            else if (eventType == nameof(UpdateNoticeStatusEvent))
            {
                var payload = JsonSerializer.Deserialize<UpdateNoticeStatusEvent>(eventPayload)!;

                notificationPayload.Add(NotificationLinkTypes.BoardId, payload.BoardId.ToString());
                notificationPayload.Add(NotificationLinkTypes.BoardName, payload.BoardName);
                notificationPayload.Add(NotificationLinkTypes.NoticeId, payload.NoticeId.ToString());
                notificationPayload.Add(NotificationLinkTypes.AccountId, payload.AccountId.ToString());
                notificationPayload.Add(NotificationLinkTypes.AccountName, payload.AccountName);
                notificationPayload.Add(NotificationLinkTypes.Status, payload.Completed.ToString());
            }
            else if (eventType == nameof(NewBoardMemberEvent))
            {
                var payload = JsonSerializer.Deserialize<NewBoardMemberEvent>(eventPayload)!;

                notificationPayload.Add(NotificationLinkTypes.BoardId, payload.BoardId.ToString());
                notificationPayload.Add(NotificationLinkTypes.BoardName, payload.BoardName);
                notificationPayload.Add(NotificationLinkTypes.AccountId, payload.AccountId.ToString());
                notificationPayload.Add(NotificationLinkTypes.AccountName, payload.AccountName);
            }
            else if (eventType == nameof(RemoveBoardMemberEvent))
            {
                var payload = JsonSerializer.Deserialize<RemoveBoardMemberEvent>(eventPayload)!;

                notificationPayload.Add(NotificationLinkTypes.BoardId, payload.BoardId.ToString());
                notificationPayload.Add(NotificationLinkTypes.BoardName, payload.BoardName);
                notificationPayload.Add(NotificationLinkTypes.AccountId, payload.RemovedAccountId.ToString());
                notificationPayload.Add(NotificationLinkTypes.AccountName, payload.RemovedAccountName);
                notificationPayload.Add(NotificationLinkTypes.SourceAccountId, payload.RemoveByAccountId.ToString());
                notificationPayload.Add(NotificationLinkTypes.SourceAccountName, payload.RemoveByAccountName);
            }
            else if (eventType == nameof(ResolveAccessRequestEvent))
            {
                var payload = JsonSerializer.Deserialize<ResolveAccessRequestEvent>(eventPayload)!;

                notificationPayload.Add(NotificationLinkTypes.BoardId, payload.BoardId.ToString());
                notificationPayload.Add(NotificationLinkTypes.BoardName, payload.BoardName);
                notificationPayload.Add(NotificationLinkTypes.SourceAccountId, payload.SourceAccountId.ToString());
                notificationPayload.Add(NotificationLinkTypes.SourceAccountName, payload.SourceAccountName);
                notificationPayload.Add(NotificationLinkTypes.Status, payload.Status.ToString());
            }
            else if (eventType == nameof(NewBoardInviteRequestEvent))
            {
                var payload = JsonSerializer.Deserialize<NewBoardInviteRequestEvent>(eventPayload)!;

                notificationPayload.Add(NotificationLinkTypes.BoardId, payload.BoardId.ToString());
                notificationPayload.Add(NotificationLinkTypes.BoardName, payload.BoardName);
                notificationPayload.Add(NotificationLinkTypes.AccountId, payload.AccountId.ToString());
                notificationPayload.Add(NotificationLinkTypes.SourceAccountId, payload.FromAccountId.ToString());
                notificationPayload.Add(NotificationLinkTypes.SourceAccountName, payload.FromAccountName);
            }
            else if (eventType == nameof(NewBoardAccessRequestEvent))
            {
                var payload = JsonSerializer.Deserialize<NewBoardAccessRequestEvent>(eventPayload)!;

                notificationPayload.Add(NotificationLinkTypes.BoardId, payload.BoardId.ToString());
                notificationPayload.Add(NotificationLinkTypes.BoardName, payload.BoardName);
                notificationPayload.Add(NotificationLinkTypes.AccountId, payload.AccountId.ToString());
                notificationPayload.Add(NotificationLinkTypes.AccountName, payload.AccountName);
            }
            else if (eventType == nameof(NewBoardMemberPermissionsEvent))
            {
                var payload = JsonSerializer.Deserialize<NewBoardMemberPermissionsEvent>(eventPayload)!;

                notificationPayload.Add(NotificationLinkTypes.BoardId, payload.BoardId.ToString());
                notificationPayload.Add(NotificationLinkTypes.BoardName, payload.BoardName);
                notificationPayload.Add(NotificationLinkTypes.AccountId, payload.AccountId.ToString());
                notificationPayload.Add(NotificationLinkTypes.AccountName, payload.AccountName);
                notificationPayload.Add(NotificationLinkTypes.SourceAccountId, payload.SourceAccountId.ToString());
                notificationPayload.Add(NotificationLinkTypes.SourceAccountName, payload.SourceAccountName);
            }
            else if (eventType == nameof(UpdateNoticeEvent))
            {
                var payload = JsonSerializer.Deserialize<UpdateNoticeEvent>(eventPayload)!;

                notificationPayload.Add(NotificationLinkTypes.BoardId, payload.BoardId.ToString());
                notificationPayload.Add(NotificationLinkTypes.BoardName, payload.BoardName);
                notificationPayload.Add(NotificationLinkTypes.AccountId, payload.AccountId.ToString());
                notificationPayload.Add(NotificationLinkTypes.AccountName, payload.AccountName);
                notificationPayload.Add(NotificationLinkTypes.NoticeId, payload.NoticeId.ToString());
                notificationPayload.Add(NotificationLinkTypes.NoticeDefinition, payload.NoticeDefinition);
            }

            return notificationPayload;
        }
    }
}
