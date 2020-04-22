using Cambridge.Demo.AuthServer.Models;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;


namespace Cambridge.Demo.AuthServer.Controllers
{
	public class ConsentController : Controller
	{
		readonly IIdentityServerInteractionService _interaction;
		readonly IResourceStore _resourceStore;
		readonly IEventService _events;
		readonly IClientStore _clientStore;

		public ConsentController(
			IIdentityServerInteractionService interaction,
			IResourceStore resourceStore,
			IEventService events,
			IClientStore clientStore)
		{
			_interaction = interaction;
			_resourceStore = resourceStore;
			_events = events;
			_clientStore = clientStore;
		}

		public async Task<IActionResult> Index(string returnUrl)
		{
			var consentViewModel = await CreateConsentViewModel(returnUrl);
			if (consentViewModel != null)
			{
				return View(consentViewModel);
			}

			return View("Error");
		}

		async Task<ConsentViewModel> CreateConsentViewModel(string returnUrl, ConsentInputModel model = null)
		{
			var consentContext = await GetConsentContext(returnUrl);
			if (consentContext is null)
				return null;

			var vm = new ConsentViewModel
			{
				RememberConsent = model?.RememberConsent ?? true,
				ScopesConsented = model?.ScopesConsented ?? Enumerable.Empty<string>(),
				ReturnUrl = returnUrl,
				ClientName = consentContext.AuthorizedClient.ClientName ?? consentContext.AuthorizedClient.ClientId,
				ClientUrl = consentContext.AuthorizedClient.ClientUri,
				AllowRememberConsent = consentContext.AuthorizedClient.AllowRememberConsent
			};

			// Only set the scopes that are checked 
			// If model is null take them all
			vm.IdentityScopes = consentContext.RequestedResources.IdentityResources.Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model is null)).ToArray();
			vm.ApiScopes = consentContext.RequestedResources.ApiResources.SelectMany(x => x.Scopes).Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model is null)).ToArray();
			
			//In case we requested the offline_access include it in the Resource Scopes
			if (consentContext.AuthorizedClient.AllowOfflineAccess && consentContext.RequestedResources.OfflineAccess)
			{
				vm.ApiScopes = vm.ApiScopes.Union(new ScopeViewModel[] {
					GetOfflineAccessScope(vm.ScopesConsented.Contains(IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess) || model == null)
				});
			}

			return vm;
		}


		ScopeViewModel GetOfflineAccessScope(bool check)
		{
			return new ScopeViewModel
			{
				Value = IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess,
				DisplayName = "Offline Access",
				Description = "Offline Access Description",
				Emphasize = true,
				Checked = check
			};
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(ConsentInputModel model)
		{
			var result = await ProcessConsent(model);
			
			if (result.IsRedirect)
			{
				//Can redirect to the Authorization Endpoint
				return Redirect(result.RedirectUri);
			}

			if (result.HasValidationError)
			{
				ModelState.AddModelError(string.Empty, result.ValidationError);
			}

			if (result.ShowView)
			{
				return View("Index", result.ViewModel);
			}

			return View("Error");
		}

		async Task<ProcessConsentResult> ProcessConsent(ConsentInputModel model)
		{
			var result = new ProcessConsentResult();

			//Validate return url is still valid
			var request = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
			if (request == null) return result;

			ConsentResponse grantedConsent = null;

			//User clicked 'no' - send back the standard 'access_denied' response
			if (model?.Button == "no")
			{
				grantedConsent = ConsentResponse.Denied;

				await _events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.ClientId, request.ScopesRequested));
			}
			//User clicked 'yes' - validate the data
			else if (model?.Button == "yes")
			{
				//If the user consented to some scope, build the response model
				if (model.ScopesConsented != null && model.ScopesConsented.Any())
				{
					var scopes = model.ScopesConsented;
					
					grantedConsent = new ConsentResponse
					{
						RememberConsent = model.RememberConsent,
						ScopesConsented = scopes.ToArray()
					};

					await _events.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request.ClientId, request.ScopesRequested, grantedConsent.ScopesConsented, grantedConsent.RememberConsent));
				}
				else
				{
					result.ValidationError = "Must Choose at least one scope";
				}
			}
			else
			{
				result.ValidationError = "No Scope Granted";
			}

			if (grantedConsent != null)
			{
				//Communicate outcome of consent back to IdentityServer
				await _interaction.GrantConsentAsync(request, grantedConsent);

				//Indicate that's it ok to redirect back to authorization endpoint
				result.RedirectUri = model.ReturnUrl;
				result.Client = await _clientStore.FindClientByIdAsync(request.ClientId);
			}
			else
			{
				//Need to redisplay the consent UI
				result.ViewModel = await CreateConsentViewModel(model.ReturnUrl, model);
			}

			return result;
		}


		ScopeViewModel CreateScopeViewModel(Scope apiScope, bool check) =>
			new ScopeViewModel
			{
				Value = apiScope.Name,
				DisplayName = apiScope.DisplayName ?? apiScope.Name,
				Description = apiScope.Description,
				Emphasize = apiScope.Emphasize,
				Required = apiScope.Required,
				Checked = check || apiScope.Required
			};

		ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check) => 
			new ScopeViewModel
			{
				Value = identity.Name,
				DisplayName = identity.DisplayName,
				Description = identity.Description,
				Emphasize = identity.Emphasize,
				Required = identity.Required,
				Checked = check || identity.Required
			};
		
		async Task<ConsentContext> GetConsentContext(string returnUrl)
		{
			var request = await _interaction.GetAuthorizationContextAsync(returnUrl);
			if (request is null)
				return null;

			return new ConsentContext()
			{
				AuthRequest = request,
				AuthorizedClient = await _clientStore.FindClientByIdAsync(request.ClientId),
				RequestedResources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested)
			};
		}

		class ConsentContext
		{
			public AuthorizationRequest AuthRequest { get; set; }
			
			public Client AuthorizedClient { get; set; }

			public Resources RequestedResources { get; set; }
		}
	}
}