using Cambridge.Demo.Client.Config;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cambridge.Demo.Client.Controllers
{
	[Route("[controller]/[action]/{documentName}")]
    public class CompanyController : ControllerBase
    {
	    readonly IHttpClientFactory _clientFactory;
	    readonly IdentitySettings _identitySettings;

	    public CompanyController(IHttpClientFactory clientFactory, IdentitySettings identitySettings)
	    {
		    _clientFactory = clientFactory;
		    _identitySettings = identitySettings;
	    }

        [HttpGet]
	    public async Task<IActionResult> GetFreeDocument(string documentName)
	    {
		    HttpResponseMessage res = await GetFreeDocumentResponse(documentName);

		    if (res.IsSuccessStatusCode)
			    return Ok(await res.Content.ReadAsStringAsync());

		    if (res.StatusCode.Equals(HttpStatusCode.Forbidden) || res.StatusCode.Equals(HttpStatusCode.Unauthorized))
		    {
			    var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
			    if (refreshToken != null)
			    {
				    HttpClient refreshTokenClient = new();

				    TokenResponse refreshResponse = await refreshTokenClient.RequestRefreshTokenAsync(new RefreshTokenRequest
				    {
					    Address = _identitySettings.TokenEndpoint,

					    ClientId = _identitySettings.ClientId,
					    ClientSecret = _identitySettings.ClientSecret,

					    RefreshToken = refreshToken
				    });

				    if (!refreshResponse.IsError)
				    {
					    AuthenticateResult authInfo = await HttpContext.AuthenticateAsync("Cookies");
					    authInfo.Properties.UpdateTokenValue(
						    "access_token",
						    refreshResponse.AccessToken);
					    authInfo.Properties.UpdateTokenValue(
						    "refresh_token",
						    refreshResponse.RefreshToken);

					    // we're signing in again with the new values.  
					    await HttpContext.SignInAsync("Cookies",
						    authInfo.Principal, authInfo.Properties);

					    res = await GetFreeDocumentResponse(documentName);
					    if (res.IsSuccessStatusCode)
						    return Ok(await res.Content.ReadAsStringAsync());
					}
				}
		    }

			if (res.StatusCode == HttpStatusCode.NotFound)
			    return NotFound($"Document {documentName} not Found");

		    return Forbid();
	    }

	    async Task<HttpResponseMessage> GetFreeDocumentResponse(string documentName)
	    {
		    HttpClient client = _clientFactory.CreateClient("apiClient");
		    var accessToken = await HttpContext.GetTokenAsync("access_token");

		    client.SetBearerToken(accessToken);
		    return  await client.GetAsync($"Document/GetFree/{documentName}");
		}


	    [HttpGet]
	    public async Task<IActionResult> GetPersonalDocument(string documentName)
	    {
		    HttpResponseMessage res = await GetPersonalDocumentResponse(documentName);

			if (res.IsSuccessStatusCode)
			    return Ok(await res.Content.ReadAsStringAsync());

			if (res.StatusCode.Equals(HttpStatusCode.Forbidden) || res.StatusCode.Equals(HttpStatusCode.Unauthorized))
			{
				//Refresh the Access Token 
				var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

				if (refreshToken != null)
				{
					HttpClient refreshTokenClient = new();

					TokenResponse refreshResponse = await refreshTokenClient.RequestRefreshTokenAsync(new RefreshTokenRequest
					{
						Address = _identitySettings.TokenEndpoint,

						ClientId = _identitySettings.ClientId,
						ClientSecret = _identitySettings.ClientSecret,

						RefreshToken = refreshToken
					});

					if (!refreshResponse.IsError)
					{
						AuthenticateResult authInfo = await HttpContext.AuthenticateAsync("Cookies");
						authInfo.Properties.UpdateTokenValue(
							"access_token",
							refreshResponse.AccessToken);
						authInfo.Properties.UpdateTokenValue(
							"refresh_token",
							refreshResponse.RefreshToken);

						//Store new tokens in the Cookies  
						await HttpContext.SignInAsync("Cookies",
							authInfo.Principal, authInfo.Properties);

						res = await GetPersonalDocumentResponse(documentName);
						if (res.IsSuccessStatusCode)
							return Ok(await res.Content.ReadAsStringAsync());
					}
				}
			}

			if (res.StatusCode == HttpStatusCode.NotFound)
			    return NotFound($"Document {documentName} not Found");

		    return Forbid();
	    }


	    async Task<HttpResponseMessage> GetPersonalDocumentResponse(string documentName)
	    {
		    HttpClient client = _clientFactory.CreateClient("apiClient");
		    var accessToken = await HttpContext.GetTokenAsync("access_token");

		    client.SetBearerToken(accessToken);
		    return await client.GetAsync($"Document/GetPersonal/{documentName}");
		}
    }
}