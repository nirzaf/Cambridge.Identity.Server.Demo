using Cambridge.Demo.Client.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cambridge.Demo.Client.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		[AllowAnonymous]
		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> AuthInfo()
		{
			AuthInfoViewModel authInfoModel = new()
            {
				AccessToken = BuildTokenViewModel(await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken)),
				IdToken = BuildTokenViewModel(await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken)),
			};

			return View(authInfoModel);
		}

		public async Task<IActionResult> ApiCalls()
		{
			return View();
		}

		static TokenViewModel BuildTokenViewModel(string token)
		{
			var tokenParts = token.Split('.');
			return tokenParts.Length switch
			{
				3 => new TokenViewModel
				{
					Header = tokenParts[0],
					Body = tokenParts[1], 
					Signature = tokenParts[2], 
					PayloadData = JObject.Parse(Encoding.UTF8.GetString(Base64Url.Decode(tokenParts[1])))
				},
				1 => new TokenViewModel{ Body = tokenParts[0]}
			};
		}
	}
}
