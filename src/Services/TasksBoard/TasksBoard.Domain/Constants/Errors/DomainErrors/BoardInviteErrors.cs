using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Domain.Constants.Errors.DomainErrors
{
    public static class BoardInviteErrors
    {
        public static readonly Error NotFound = new(ErrorCodes.NotFound, $"Board invite request was not found.");
        public static Error AlreadyExist(string boardName) => new(ErrorCodes.AlreadyExist, $"Invite request is already exist for board '{boardName}'.");
        public static Error CantCreate(string boardName) => new(ErrorCodes.CantCreate, $"Can't create new invite request to board '{boardName}'.");
        public static Error CantCancel(string boardName) => new(ErrorCodes.CantCancel, $"Can't cancel invite request to board '{boardName}'.");
    }
}
