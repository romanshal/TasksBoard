using Authentication.Application.Dtos;
using Authentication.Application.Features.Authentications.Commands.RefreshToken;
using Authentication.Application.Handlers;
using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Secutiry;
using Common.Blocks.Models.DomainResults;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Authentication.Tests.Units.Application.Features.Authentications
{
    public class RefreshTokenCommandHandlerShould
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly ISetup<UserManager<ApplicationUser>, Task<ApplicationUser>> _userManagerFindSetup;
        private readonly Mock<ISignInHandler> _signInHandler;
        private readonly ISetup<ISignInHandler, Task<Result<AuthenticationDto>>> _signInHandlerSetup;
        private readonly Mock<ILogger<RefreshTokenCommandHandler>> _logger;
        private readonly RefreshTokenCommandHandler _sut;

        public RefreshTokenCommandHandlerShould()
        {
            _userManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _userManagerFindSetup = _userManager
                .Setup(s => s.FindByIdAsync(It.IsAny<string>()));

            _signInHandler = new Mock<ISignInHandler>();
            _signInHandlerSetup = _signInHandler
                .Setup(s => s.HandleAsync(
                    It.IsAny<ApplicationUser>(), 
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<CancellationToken>()));

            _logger = new Mock<ILogger<RefreshTokenCommandHandler>>();

            _sut = new RefreshTokenCommandHandler(_userManager.Object, _signInHandler.Object, _logger.Object);
        }

        [Fact]
        public async Task ReturnSuccessResult_WhenTokenRefreshed()
        {
            var userId = Guid.Parse("193c2bae-715f-45be-826d-92d299710fbf");
            var command = new RefreshTokenCommand 
            {
                UserIp = "0.0.0.1",
                UserAgent = string.Empty,
                UserId = userId,
                RefreshToken = string.Empty,
                DeviceId = Guid.Parse("93eef0e4-b4ef-4b5a-bf11-4920d80ef5a9").ToString()
            };

            _userManagerFindSetup.ReturnsAsync(new ApplicationUser { Id = userId });

            var authenticationDto = new AuthenticationDto
            {
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                AccessTokenExpiredAt = DateTime.UtcNow,
                RefreshTokenExpiredAt = DateTime.UtcNow,
                UserId = userId,
                DeviceId = Guid.NewGuid().ToString(),
            };

            _signInHandlerSetup.ReturnsAsync(authenticationDto);

            var actual = await _sut.Handle(command, CancellationToken.None);
            actual.IsSuccess.Should().BeTrue();
            actual.Value.Should().NotBeNull().And.Be(authenticationDto);
        }

        [Fact]
        public async Task ReturnNotFoundResult_WhenUserNotFound()
        {
            var userId = Guid.Parse("193c2bae-715f-45be-826d-92d299710fbf");
            var command = new RefreshTokenCommand
            {
                UserIp = "0.0.0.1",
                UserAgent = string.Empty,
                UserId = userId,
                RefreshToken = string.Empty,
                DeviceId = Guid.Parse("93eef0e4-b4ef-4b5a-bf11-4920d80ef5a9").ToString()
            };

            _userManagerFindSetup.ReturnsAsync((ApplicationUser)null);

            var actual = await _sut.Handle(command, CancellationToken.None);
            actual.IsSuccess.Should().BeFalse();
            actual.Error.Should().NotBeNull().And.Be(AuthenticationErrors.UserNotFound());
        }
    }
}
