using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Domain.Constants.Errors.DomainErrors
{
    public static class BoardAccessErrors
    {
        public static readonly Error NotFound = new(ErrorCodes.NotFound, $"Board access request was not found.");
        public static readonly Error CantCancel = new(ErrorCodes.CantCancel, $"Can't cancel board access request.");
        public static Error AlreadyExist(string boardName) => new(ErrorCodes.AlreadyExist, $"Access request is already exist for board '{boardName}'.");
    }
}
