using Authentication.API.Models;
using Authentication.Domain.Entities;
using IdentityServer8.Events;
using IdentityServer8.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Controllers
{
    [AllowAnonymous]
    [Route("/")]
    public class AuthenticationController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IIdentityServerInteractionService interactionService,
            IEventService events) : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IIdentityServerInteractionService _interactionService = interactionService;

        [HttpGet]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(string returnUrl)
        {
            var viewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalProviders = await _signInManager.GetExternalAuthenticationSchemesAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(LoginViewModel viewModel)
        {
            var context = await _interactionService.GetAuthorizationContextAsync(viewModel.ReturnUrl);

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid model");

                return View(viewModel);
            }

            var user = await _userManager.FindByNameAsync(viewModel.Username);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found");

                return View(viewModel);
            }

            var result = await _signInManager.PasswordSignInAsync(viewModel.Username, viewModel.Password, false, false);
            if (result.Succeeded)
            {
                await events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName, clientId: context?.Client.ClientId));

                return Redirect(viewModel.ReturnUrl);
            }

            ModelState.AddModelError(string.Empty, "Login fault");

            await events.RaiseAsync(new UserLoginFailureEvent(viewModel.Username, "invalid credentials", clientId: context?.Client.ClientId));

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync()
        {
            return Ok();
        }

        //[HttpPost]
        //[Authorize]
        //[Route("register")]
        //public async Task<IActionResult> LogoutAsync()
        //{

        //}
    }
}
