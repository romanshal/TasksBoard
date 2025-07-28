using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Domain.Constants.Errors.DomainErrors
{
    public static class BoardPermissionErrors
    {
        public static readonly Error NoPermissions = new(ErrorCodes.NoEntities, "No permissions available.");
    }
}
