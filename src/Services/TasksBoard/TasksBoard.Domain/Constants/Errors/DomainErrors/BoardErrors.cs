using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Domain.Constants.Errors.DomainErrors
{
    public static class BoardErrors
    {
        public static readonly Error NotFound = new(ErrorCodes.NotFound, $"Board was not found.");

        public static readonly Error CantCreate = new(ErrorCodes.CantCreate, "Can't create new board.");

        public static readonly Error CantUpdate = new(ErrorCodes.CantUpdate, "Can't update board.");

        public static readonly Error CantDelete = new(ErrorCodes.CantUpdate, "Can't delete board.");

        public static readonly Error NoBoards = new(ErrorCodes.NoEntities, "No boards.");
        public static Error Private(string boardName) => new(ErrorCodes.Forbidden, $"Board '{boardName} is private.");
    }
}
