using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Domain.Constants.Errors.DomainErrors
{
    public static class BoardNoticeErrors
    {
        public static readonly Error CantCreate = new(ErrorCodes.CantCreate, "Can't create new board notice.");
        public static readonly Error NotFound = new(ErrorCodes.NotFound, "Board notice was not found.");
        public static readonly Error CantDelete = new(ErrorCodes.CantDelete, "Can't delete board notice.");
        public static readonly Error CantUpdate = new(ErrorCodes.CantDelete, "Can't update board notice.");
    }
}
