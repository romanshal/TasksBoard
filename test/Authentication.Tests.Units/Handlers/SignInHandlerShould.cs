using Authentication.Application.Dtos;
using Authentication.Application.Handlers;
using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Secutiry;
using Authentication.Domain.Models;
using Common.Blocks.Models.DomainResults;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Authentication.Tests.Units.Handlers
{
    public class SignInHandlerShould
    {
        private readonly Mock<ITokenManager> _tokenManager;
        private readonly ISetup<ITokenManager, Task<(TokenPairModel tokens, string deviceId)>> _tokenManagerIssueSetup;
        private readonly ISetup<ITokenManager, Task<TokenPairModel>> _tokenManagerRotateSetup;
        private readonly Mock<ILogger<SignInHandler>> _logger;
        private readonly SignInHandler _sut;

        public SignInHandlerShould()
        {
            _tokenManager = new Mock<ITokenManager>();
            _tokenManagerIssueSetup = _tokenManager
                .Setup(s => s.IssueAsync(It.IsAny<GenerateTokensModel>(), It.IsAny<CancellationToken>()));
            _tokenManagerRotateSetup = _tokenManager
                .Setup(s => s.RotateAsync(It.IsAny<GenerateTokensModel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
            _logger = new Mock<ILogger<SignInHandler>>();

            _sut = new SignInHandler(_tokenManager.Object, _logger.Object);
        }

        [Fact]
        public async Task ReturnValidAuthenticationDto_WhenSigninSuccess()
        {
            var userIp = "0.0.0.1";
            var userAgent = "TestAgnet";
            var user = new ApplicationUser
            {
                Id = Guid.Parse("93eef0e4-b4ef-4b5a-bf11-4920d80ef5a9")
            };

            var tokenPair = new TokenPairModel
            {
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                AccessTokenExpiredAt = DateTime.UtcNow,
                RefreshTokenExpiredAt = DateTime.UtcNow,
            };

            var deviceId = Guid.Parse("332bb7fa-cd78-4fe9-a4d7-475ae374a019").ToString();

            _tokenManagerIssueSetup.ReturnsAsync((tokenPair, deviceId));

            var authenticationDto = new AuthenticationDto
            {
                AccessToken = tokenPair.AccessToken,
                AccessTokenExpiredAt = tokenPair.AccessTokenExpiredAt,
                RefreshToken = tokenPair.RefreshToken,
                RefreshTokenExpiredAt = tokenPair.RefreshTokenExpiredAt,
                UserId = user.Id,
                DeviceId = deviceId
            };

            var actual = await _sut.HandleAsync(user, userAgent, userIp, CancellationToken.None);
            actual.IsSuccess.Should().BeTrue();
            actual.Error.Should().BeEquivalentTo(Error.None);
            actual.Value.Should().NotBeNull().And.BeEquivalentTo(authenticationDto);
        }

        [Fact]
        public async Task ReturnValidAuthenticationDto_WhenRefreshTokenSuccess()
        {
            var userIp = "0.0.0.1";
            var userAgent = "TestAgnet";
            var user = new ApplicationUser
            {
                Id = Guid.Parse("93eef0e4-b4ef-4b5a-bf11-4920d80ef5a9")
            };
            var refreshToken = Guid.Parse("332bb7fa-cd78-4fe9-a4d7-475ae374a019").ToString();
            var deviceId = Guid.Parse("332bb7fa-cd78-4fe9-a4d7-475ae374a019").ToString();

            var tokenPair = new TokenPairModel
            {
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                AccessTokenExpiredAt = DateTime.UtcNow,
                RefreshTokenExpiredAt = DateTime.UtcNow,
            };

            _tokenManagerRotateSetup.ReturnsAsync(tokenPair);

            var authenticationDto = new AuthenticationDto
            {
                AccessToken = tokenPair.AccessToken,
                AccessTokenExpiredAt = tokenPair.AccessTokenExpiredAt,
                RefreshToken = tokenPair.RefreshToken,
                RefreshTokenExpiredAt = tokenPair.RefreshTokenExpiredAt,
                UserId = user.Id,
                DeviceId = deviceId
            };

            var actual = await _sut.HandleAsync(user, userAgent, userIp, refreshToken, deviceId, CancellationToken.None);
            actual.IsSuccess.Should().BeTrue();
            actual.Error.Should().BeEquivalentTo(Error.None);
            actual.Value.Should().NotBeNull().And.BeEquivalentTo(authenticationDto);
        }

        [Fact]
        public async Task ReturnSigninFaultedResult_WhenInvalidTokenCreated_WhenSignin()
        {
            var userIp = "0.0.0.1";
            var userAgent = "TestAgnet";
            var user = new ApplicationUser
            {
                Id = Guid.Parse("93eef0e4-b4ef-4b5a-bf11-4920d80ef5a9")
            };

            TokenPairModel tokenPair = null;

            var deviceId = Guid.Parse("332bb7fa-cd78-4fe9-a4d7-475ae374a019").ToString();

            _tokenManagerIssueSetup.ReturnsAsync((tokenPair, deviceId));

            var actual = await _sut.HandleAsync(user, userAgent, userIp, CancellationToken.None);
            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().BeEquivalentTo(AuthenticationErrors.SigninFaulted);
        }

        [Fact]
        public async Task ReturnSigninFaultedResult_WhenInvalidTokenCreated_WhenRefreshToken()
        {
            var userIp = "0.0.0.1";
            var userAgent = "TestAgnet";
            var user = new ApplicationUser
            {
                Id = Guid.Parse("93eef0e4-b4ef-4b5a-bf11-4920d80ef5a9")
            };
            var refreshToken = Guid.Parse("332bb7fa-cd78-4fe9-a4d7-475ae374a019").ToString();
            var deviceId = Guid.Parse("332bb7fa-cd78-4fe9-a4d7-475ae374a019").ToString();

            TokenPairModel tokenPair = null;

            _tokenManagerRotateSetup.ReturnsAsync(tokenPair);

            var actual = await _sut.HandleAsync(user, userAgent, userIp, refreshToken, deviceId, CancellationToken.None);
            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().BeEquivalentTo(AuthenticationErrors.SigninFaulted);
        }
    }
}
