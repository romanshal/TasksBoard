using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;

namespace Authentication.Domain.Constants.AuthenticationErrors
{
    public static class ExternalAuthenticationErrors
    {
        /// <summary>
        /// External login info not found.
        /// </summary>
        public static readonly Error LoginNotFound = new(ErrorCodes.NotFound, "External login info not found.");

        /// <summary>
        /// External login info not found.
        /// </summary>
        public static readonly Error ProviderNotFound = new(ErrorCodes.NoEntities, "External login info not found.");

        /// <summary>
        /// Linked user not found.
        /// </summary>
        public static readonly Error UserNotFound = new(ErrorCodes.NotFound, "Linked user not found.");

        /// <summary>
        /// Account with this email already exists. Please sign in and link the provider from your profile.
        /// </summary>
        public static readonly Error AlreadyExist = new(ErrorCodes.AlreadyExist, "Account with this email already exists. Please sign in and link the provider from your profile.");

        /// <summary>
        /// External login failed.
        /// </summary>
        public static readonly Error CantCreate = new(ErrorCodes.CantCreate, "External login failed.");
        
        /// <summary>
        /// Invalid
        /// </summary>
        /// <param name="description">Description</param>
        /// <returns></returns>
        public static Error Invalid(string description) => new(ErrorCodes.Invalid, description);
    }
}
