using Cambridge.Demo.AuthServer.Models;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace Cambridge.Demo.AuthServer.Controllers
{
	public class AccountController : Controller
    {
	    readonly IEventService _eventService;
	    readonly IIdentityServerInteractionService _interaction;
	    readonly TestUserStore _userStore;

	    public AccountController(
			IEventService eventService,
		    IIdentityServerInteractionService interaction,
			TestUserStore userStore)
		{
			_eventService = eventService;
			_interaction = interaction;
			_userStore = userStore;
		}


		
		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login(string returnUrl)
		{
			LoginViewModel model = new LoginViewModel();
			model.ReturnUrl = returnUrl;

			return View(model);
		}

		[HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
	        AuthorizationRequest context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
	        if (ModelState.IsValid)
	        {
		        if (_userStore.ValidateCredentials(model.Email,model.Password))
		        {
			        TestUser user = _userStore.FindByUsername(model.Email);

					IdentityServerUser identityUser = new IdentityServerUser(user.SubjectId)
					{
						DisplayName = user.Username,
						AdditionalClaims = user.Claims
					};

					await HttpContext.SignInAsync(identityUser.CreatePrincipal());

			        await _eventService.RaiseAsync(new UserLoginSuccessEvent(user.Username, user.SubjectId, user.Username));

					if (context != null)
				        return Redirect(model.ReturnUrl);
		        }
	        }

	        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
	        return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
	        LogoutRequest logout = await _interaction.GetLogoutContextAsync(logoutId);

			LogoutViewModel vm = new LogoutViewModel
	        {
				PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
				SignOutIframeUrl = logout.SignOutIFrameUrl,
				LogoutId = logoutId
	        };

			if (User?.Identity.IsAuthenticated == true)
			{
				await _eventService.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));

				await HttpContext.SignOutAsync();
			}

			return View(vm);
		}

    }
}