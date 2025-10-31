using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;

namespace Authentication.Domain.Constants.AuthenticationErrors
{
    public class AuthenticationErrors
    {
        /// <summary>
        /// Your account is temporarily locked. Please contact support for assistance.
        /// </summary>
        public static readonly Error Locked = new(ErrorCodes.Locked, "Your account is temporarily locked. Please contact support for assistance.");

        /// <summary>
        /// "Signin is not allowed for you. Please contact support for assistance.
        /// </summary>
        public static readonly Error NotAllowed = new(ErrorCodes.NotAllowed, "Signin is not allowed for you. Please contact support for assistance.");

        /// <summary>
        /// The username or password you entered is incorrect. Please try again.
        /// </summary>
        public static readonly Error Invalid = new(ErrorCodes.Invalid, "The username or password you entered is incorrect. Please try again.");

        /// <summary>
        /// Signin faulted. Please try again later.
        /// </summary>
        public static readonly Error SigninFaulted = new(ErrorCodes.Invalid, "Signin faulted. Please try again later.");

        /// <summary>
        /// Signup faulted. Please try again later.
        /// </summary>
        public static readonly Error SignupFaulted = new(ErrorCodes.Invalid, "Signup faulted. Please try again later.");

        /// <summary>
        /// Email confirmation required.
        /// </summary>
        public static readonly Error EmailNotConfirmed = new(ErrorCodes.NotAllowed, "Email confirmation required.");

        /// <summary>
        /// User with name {username} is already exist.
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns></returns>
        public static Error AlreadyExist(string username) => new(ErrorCodes.AlreadyExist, $"User with name {username} is already exist.");

        /// <summary>
        /// User with name {username} not found.
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns></returns>
        public static Error UserNotFound(string username) => new(ErrorCodes.NotFound, $"User with name {username} not found.");

        /// <summary>
        /// User not found.
        /// </summary>
        /// <returns></returns>
        public static Error UserNotFound() => new(ErrorCodes.NotFound, $"User not found.");
    }
}
