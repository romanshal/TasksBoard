using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;

namespace Authentication.Domain.Constants.ManageErrors
{
    public static class ManageErrors
    {
        public static readonly Error UserNotFound = new(ErrorCodes.NotFound, "User not found.");
        public static readonly Error InvalidPassword = new(ErrorCodes.Invalid, "Invalid password.");
        public static readonly Error CantUpdatePassword = new(ErrorCodes.CantUpdate, "Can't update user password.");
        public static readonly Error CantUpdateImage = new(ErrorCodes.CantUpdate, "Can't update user image.");
        public static readonly Error CantUpdateInfo = new(ErrorCodes.CantUpdate, "Can't update user info.");
    }
}
