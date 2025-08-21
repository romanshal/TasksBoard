using Authentication.Application.Features.Authentications.Commands.RefreshToken;
using Authentication.Application.Interfaces.Services;
using Authentication.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

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
    }
}
