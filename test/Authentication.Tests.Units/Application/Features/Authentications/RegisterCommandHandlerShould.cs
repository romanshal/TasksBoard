using Authentication.Application.Features.Authentications.Commands.Register;
using Authentication.Application.Interfaces.Services;
using Authentication.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Authentication.Tests.Units.Application.Features.Authentications
{
    public class RegisterCommandHandlerShould
    {
        private readonly Mock<UserManager<ApplicationUser>> userManager;
        private readonly Mock<SignInManager<ApplicationUser>> signinManager;
        private readonly Mock<ITokenService> tokenService;
        private readonly Mock<ILogger<RegisterCommandHandler>> logger;
        private readonly RegisterCommandHandler sut;

        public RegisterCommandHandlerShould()
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

            logger = new Mock<ILogger<RegisterCommandHandler>>();

            sut = new RegisterCommandHandler(userManager.Object, signinManager.Object, tokenService.Object, logger.Object);
        }
    }
}
