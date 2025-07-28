using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;

namespace Notification.Domain.Constants
{
    public static class NotificationErrors
    {
        public static readonly Error CantUpdate = new(ErrorCodes.CantUpdate, "Can't update notifications.");
    }
}
