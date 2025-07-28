using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Domain.Constants.Errors.DomainErrors
{
    public static class BoardInviteErrors
    {
        public static Error AlreadyExist(string boardName) => new(ErrorCodes.AlreadyExist, $"Invite request is already exist for board '{boardName}'.");
        public static Error CantCreate(string boardName) => new(ErrorCodes.CantCreate, $"Can't create new access request to board '{boardName}'.");
    }
}
