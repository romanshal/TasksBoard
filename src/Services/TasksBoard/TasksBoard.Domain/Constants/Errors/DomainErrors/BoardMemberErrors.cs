using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Domain.Constants.Errors.DomainErrors
{
    public static class BoardMemberErrors
    {
        public static Error AlreadyExist(string boardName) => new(ErrorCodes.AlreadyExist, $"Board member is already exist for board '{boardName}'.");
        public static readonly Error CantCreate = new(ErrorCodes.CantCreate, "Can't create new board member.");
        public static readonly Error CantDelete = new(ErrorCodes.CantDelete, "Can't delete board member.");
        public static readonly Error NotFound = new(ErrorCodes.NotFound, "Board member was not found.");
    }
}
