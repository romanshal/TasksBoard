using Authentication.Application.Dtos;
using Authentication.Domain.Entities;
using Common.Blocks.Models.DomainResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Application.Handlers
{
    /// <summary>
    /// Facad for generate token pair (access and refresh tokens).
    /// </summary>
    /// <param name="tokenManager">Token manager.</param>
    /// <param name="logger">Logger.</param>
    public interface ISignInHandler
    {
        /// <summary>
        /// Use for signin.
        /// </summary>
        /// <param name="user">Application user.</param>
        /// <param name="userAgent">User Agent.</param>
        /// <param name="userIp">User ip.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Authentication credentials.</returns>
        Task<Result<AuthenticationDto>> HandleAsync(
            ApplicationUser user,
            string userAgent,
            string userIp,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Use for refresh access token by refresh token.
        /// </summary>
        /// <param name="user">Application user.</param>
        /// <param name="userAgent">User Agent.</param>
        /// <param name="userIp">User ip.</param>
        /// <param name="refreshToken">Refresh token.</param>
        /// <param name="deviceId">Device id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Authentication credentials.</returns>
        Task<Result<AuthenticationDto>> HandleAsync(
            ApplicationUser user,
            string userAgent,
            string userIp,
            string refreshToken,
            string deviceId,
            CancellationToken cancellationToken = default);
    }
}
