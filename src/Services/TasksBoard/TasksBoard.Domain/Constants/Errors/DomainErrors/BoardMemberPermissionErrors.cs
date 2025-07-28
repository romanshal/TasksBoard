using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Domain.Constants.Errors.DomainErrors
{
    public static class BoardMemberPermissionErrors
    {
        public static readonly Error CantCreate = new(ErrorCodes.CantCreate, "Can't save new board member permissions.");
    }
}
