using Authentication.Application.Dtos;
using Authentication.Application.Features.Authentications.Commands.Login;
using Authentication.Application.Features.Authentications.Commands.RefreshToken;
using Authentication.Application.Interfaces.Services;
using Authentication.Domain.Entities;
using Common.Blocks.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;

namespace Authentication.Tests.Units.Application.Features.Authentications
{
    public class RefreshTokenCommandHandlerShould
    {
        private readonly Mock<UserManager<ApplicationUser>> userManager;
        private readonly Mock<ITokenService> tokenService;
        private readonly Mock<ILogger<RefreshTokenCommandHandler>> logger;
        private readonly RefreshTokenCommandHandler sut;

        public RefreshTokenCommandHandlerShould()
        {
            userManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            tokenService = new Mock<ITokenService>();

            logger = new Mock<ILogger<RefreshTokenCommandHandler>>();

            sut = new RefreshTokenCommandHandler(userManager.Object, tokenService.Object, logger.Object);
        }

        [Fact]
        public async Task ReturnAuthenticatedModel_WhenTokenRefreshed()
        {
            var userId = Guid.Parse("921aec96-7a04-495a-93ae-4e989cacf682");
            var userName = "Test";
            var command = new RefreshTokenCommand
            {
                UserId = userId,
                RefreshToken = "921aec96-7a04-495a-93ae-4e989cacf682"
            };
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = userName
            };
            var token = new TokenDto
            {
                AccessToken = "ddf564f1-6cb9-4337-9a4c-5956e7189f53",
                RefreshToken = "ddf564f1-6cb9-4337-9a4c-5956e7189f53"
            };

            userManager
                .Setup(s => s.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            tokenService
                .Setup(s => s.RefreshTokenAsync(user))
                .ReturnsAsync(token);

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.Should().NotBeNull();
            actual.AccessToken.Should().NotBeNullOrWhiteSpace();
            actual.RefreshToken.Should().NotBeNullOrWhiteSpace();
            actual.UserId.Should().NotBeEmpty().And.Be(userId);
        }

        [Fact]
        public async Task ThrowUnauthorizedException_WhenUserNotFound()
        {
            var userId = Guid.Parse("921aec96-7a04-495a-93ae-4e989cacf682");
            var command = new RefreshTokenCommand
            {
                UserId = userId,
                RefreshToken = "921aec96-7a04-495a-93ae-4e989cacf682"
            };

            userManager
                .Setup(s => s.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(value: null);

            await sut
                .Invoking(s => s.Handle(command, CancellationToken.None))
                .Should()
                .ThrowAsync<UnauthorizedException>()
                .WithMessage($"User was not found.");
        }

        [Fact]
        public async Task ThrowUnauthorizedException_WhenCantCreateRefreshedToken()
        {
            var userId = Guid.Parse("921aec96-7a04-495a-93ae-4e989cacf682");
            var userName = "Test";
            var command = new RefreshTokenCommand
            {
                UserId = userId,
                RefreshToken = "921aec96-7a04-495a-93ae-4e989cacf682"
            };
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = userName
            };

            userManager
                .Setup(s => s.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            tokenService
                .Setup(s => s.RefreshTokenAsync(user))
                .ReturnsAsync(new TokenDto { AccessToken = string.Empty, RefreshToken = string.Empty });

            await sut
                .Invoking(s => s.Handle(command, CancellationToken.None))
                .Should()
                .ThrowAsync<UnauthorizedException>()
                .WithMessage("Can't create access or refresh tokens.");
        }
    }
}
