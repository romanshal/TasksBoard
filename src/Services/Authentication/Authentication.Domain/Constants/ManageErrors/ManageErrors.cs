using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;

namespace Authentication.Domain.Constants.ManageErrors
{
    public static class ManageErrors
    {
        /// <summary>
        /// User not found.
        /// </summary>
        public static readonly Error UserNotFound = new(ErrorCodes.NotFound, "User not found.");

        /// <summary>
        /// Invalid password.
        /// </summary>
        public static readonly Error InvalidPassword = new(ErrorCodes.Invalid, "Invalid password.");

        /// <summary>
        /// Can't update user password.
        /// </summary>
        public static readonly Error CantUpdatePassword = new(ErrorCodes.CantUpdate, "Can't update user password.");

        /// <summary>
        /// Can't update user image.
        /// </summary>
        public static readonly Error CantUpdateImage = new(ErrorCodes.CantUpdate, "Can't update user image.");

        /// <summary>
        /// Can't update user info.
        /// </summary>
        public static readonly Error CantUpdateInfo = new(ErrorCodes.CantUpdate, "Can't update user info.");
    }
}
