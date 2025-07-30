using Authentication.Application.Dtos;
using Authentication.Application.Features.Authentications.Commands.Login;
using Authentication.Application.Interfaces.Services;
using Authentication.Domain.Entities;
using Common.Blocks.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Authentication.Tests.Units.Application.Features.Authentications
{
    public class LoginCommandHandlerShould
    {
        private readonly Mock<UserManager<ApplicationUser>> userManager;
        private readonly Mock<SignInManager<ApplicationUser>> signinManager;
        private readonly Mock<ITokenService> tokenService;
        private readonly Mock<ILogger<LoginCommandHandler>> logger;
        private readonly LoginCommandHandler sut;

        public LoginCommandHandlerShould()
        {
            userManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            signinManager = new Mock<SignInManager<ApplicationUser>>(
                userManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<ILogger<SignInManager<ApplicationUser>>>(),
                Mock.Of<IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<ApplicationUser>>());

            tokenService = new Mock<ITokenService>();

            logger = new Mock<ILogger<LoginCommandHandler>>();

            sut = new LoginCommandHandler(userManager.Object, signinManager.Object, tokenService.Object, logger.Object);
        }

        [Fact]
        public async Task ReturnAuthenticatedModel_WhenValidAuthenticationCredentionals()
        {
            var userId = Guid.Parse("ddf564f1-6cb9-4337-9a4c-5956e7189f53");
            var userName = "Test";
            var command = new LoginCommand
            {
                Username = userName,
                Password = string.Empty
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
                .Setup(s => s.FindByNameAsync(userName))
                .ReturnsAsync(user);

            signinManager
                .Setup(s => s.PasswordSignInAsync(userName, It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            tokenService
                .Setup(s => s.GenerateTokenAsync(user))
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
            var userName = "Test";
            var command = new LoginCommand
            {
                Username = userName,
                Password = string.Empty
            };

            userManager
                .Setup(s => s.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(value: null);

            await sut
                .Invoking(s => s.Handle(command, CancellationToken.None))
                .Should()
                .ThrowAsync<UnauthorizedException>()
                .WithMessage($"User with name {userName} not found.");
        }

        [Fact]
        public async Task ThrowSigninFaultedException_WhenPasswordSigninFaulted()
        {
            var userName = "Test";
            var command = new LoginCommand
            {
                Username = userName,
                Password = string.Empty
            };
            var user = new ApplicationUser
            {
                Id = Guid.Empty,
                UserName = string.Empty
            };

            userManager
                .Setup(s => s.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            signinManager
                .Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed);

            await sut
                .Invoking(s => s.Handle(command, CancellationToken.None))
                .Should()
                .ThrowAsync<SigninFaultedException>()
                .WithMessage($"The username or password you entered is incorrect. Please try again.");
        }

        [Fact]
        public async Task ThrowLockedException_WhenSigninLocked()
        {
            var userName = "Test";
            var command = new LoginCommand
            {
                Username = userName,
                Password = string.Empty
            };
            var user = new ApplicationUser
            {
                Id = Guid.Empty,
                UserName = string.Empty
            };

            userManager
                .Setup(s => s.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            signinManager
                .Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.LockedOut);

            await sut
                .Invoking(s => s.Handle(command, CancellationToken.None))
                .Should()
                .ThrowAsync<LockedException>()
                .WithMessage($"Your account is temporarily locked. Please contact support for assistance.");
        }

        [Fact]
        public async Task ThrowNotAllowedException_WhenSigninNotAllowed()
        {
            var userName = "Test";
            var command = new LoginCommand
            {
                Username = userName,
                Password = string.Empty
            };
            var user = new ApplicationUser
            {
                Id = Guid.Empty,
                UserName = string.Empty
            };

            userManager
                .Setup(s => s.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            signinManager
                .Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.NotAllowed);

            await sut
                .Invoking(s => s.Handle(command, CancellationToken.None))
                .Should()
                .ThrowAsync<NotAllowedException>()
                .WithMessage($"Signin is not allowed for user {userName}.");
        }

        [Fact]
        public async Task ThrowInvalidOperationException_WhenCantCreateToken()
        {
            var userId = Guid.Parse("ddf564f1-6cb9-4337-9a4c-5956e7189f53");
            var userName = "Test";
            var command = new LoginCommand
            {
                Username = userName,
                Password = string.Empty
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
                .Setup(s => s.FindByNameAsync(userName))
                .ReturnsAsync(user);

            signinManager
                .Setup(s => s.PasswordSignInAsync(userName, It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            tokenService
                .Setup(s => s.GenerateTokenAsync(user))
                .ReturnsAsync(new TokenDto
                {
                    AccessToken = string.Empty,
                    RefreshToken = string.Empty
                });

            await sut
                .Invoking(s => s.Handle(command, CancellationToken.None))
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Can't create access or refresh tokens.");
        }
    }
}
