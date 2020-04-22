using Cambridge.Demo.Client.Config;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cambridge.Demo.Client.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		readonly IdentitySettings _identitySettings;

		public AccountController(IdentitySettings identitySettings)
		{
			_identitySettings = identitySettings;
		}

		public IActionResult Login()
		{
			return RedirectToAction("Index", "Home");
		}

		public async Task Logout()
		{
			var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

			if (refreshToken != null)
			{
				var httpClient = new HttpClient();

				var revokationResult = await httpClient.RevokeTokenAsync(new TokenRevocationRequest
				{
					Address = _identitySettings.RevocationEndpoint,
					ClientId = _identitySettings.ClientId,
					ClientSecret = _identitySettings.ClientSecret,
					Token = refreshToken,
				});
				if (!revokationResult.IsError)
				{
					await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
					await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
				}
			}

			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
		}

		[AllowAnonymous]
		public IActionResult AccessDenied(string error)
		{
			ViewData["Error"] = error;
			return View();
		}
	}
}