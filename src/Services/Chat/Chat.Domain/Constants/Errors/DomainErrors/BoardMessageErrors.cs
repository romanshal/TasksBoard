using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;

namespace Chat.Domain.Constants.Errors.DomainErrors
{
    public static class BoardMessageErrors
    {
        public static readonly Error NotFound = new(ErrorCodes.NotFound, $"Message was not found.");
        public static readonly Error CantCreate = new(ErrorCodes.CantCreate, "Can't create new message.");
        public static readonly Error CantUpdate = new(ErrorCodes.CantUpdate, "Can't update message.");
        public static readonly Error CantDelete = new(ErrorCodes.CantUpdate, "Can't delete message.");
    }
}
