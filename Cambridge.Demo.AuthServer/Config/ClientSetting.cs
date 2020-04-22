using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Cambridge.Demo.AuthServer.Config
{
	/// <summary>
	/// Create a Client Settings based on the appsettings
	/// </summary>
	public class ClientSetting : Client
	{
		const string ClientIdKey = "ClientId";
		const string ClientSecretKey = "ClientSecret";
		const string AbsoluteRefreshTokenLifeTimeKey = "AbsoluteRefreshTokenLifeTime";
		const string SlidingRefreshTokenLifetimeKey = "SlidingRefreshTokenLifetime";
		const string RefreshTokenUsageKey = "RefreshTokenUsage";
		const string UpdateAccessTokenClaimsOnRefreshKey = "UpdateAccessTokenClaimsOnRefresh";
		const string AllowOfflineAccessKey = "AllowOfflineAccess";
		const string ClientNameKey = "ClientName";
		const string AccessTokenLifeTimeKey = "AccessTokenLifetime";
		const string AllowedGrantTypesKey = "AllowedGrantTypes";
		const string RequireClientSecretKey = "RequireClientSecret";
		const string AllowedScopesKey = "AllowedScopes";
		const string RefreshTokenExpirationKey = "RefreshTokenExpiration";
		const string RedirectUrisKey = "RedirectUris";
		const string PostLogoutRedirectUrisKey = "PostLogoutRedirectUris";
		const string AlwaysIncludeUserClaimsInIdTokenKey = "AlwaysIncludeUserClaimsInIdToken";
		const string RequireConsentKey = "RequireConsent";
		const string BackChannelLogoutKey = "BackChannelLogout";

		public ClientSetting(IConfigurationSection configuration)
		{
			ClientId = configuration[ClientIdKey];
			ClientSecrets = configuration[ClientSecretKey].Split(",").Select(x => new Secret(x.Sha256())).ToList();
			
			UpdateAccessTokenClaimsOnRefresh = !string.IsNullOrEmpty(configuration[UpdateAccessTokenClaimsOnRefreshKey]) && bool.Parse(configuration[UpdateAccessTokenClaimsOnRefreshKey]);
			
			AllowOfflineAccess = !string.IsNullOrEmpty(configuration[AllowOfflineAccessKey]) && bool.Parse(configuration[AllowOfflineAccessKey]);
			
			ClientName = configuration[ClientNameKey];
			
			AccessTokenLifetime = StringToIntMultiplication(configuration[AccessTokenLifeTimeKey]);

			if (!string.IsNullOrEmpty(configuration[AbsoluteRefreshTokenLifeTimeKey]))
			{
				AbsoluteRefreshTokenLifetime = StringToIntMultiplication(configuration[AbsoluteRefreshTokenLifeTimeKey]);
			}

			if (!string.IsNullOrEmpty(configuration[SlidingRefreshTokenLifetimeKey]))
			{
				SlidingRefreshTokenLifetime = StringToIntMultiplication(configuration[SlidingRefreshTokenLifetimeKey]);
			}

			if (!string.IsNullOrEmpty(configuration[RefreshTokenUsageKey]))
			{
				RefreshTokenUsage = configuration[RefreshTokenUsageKey].Equals("Reuse") ? TokenUsage.ReUse : TokenUsage.OneTimeOnly;
			}

			AllowedGrantTypes = configuration[AllowedGrantTypesKey].Split(",");
			RequireClientSecret = bool.Parse(configuration[RequireClientSecretKey]);
			AllowedScopes = configuration[AllowedScopesKey].Trim().Split(",").ToList();

			if (!string.IsNullOrEmpty(configuration[RefreshTokenExpirationKey]))
			{
				RefreshTokenExpiration = configuration[RefreshTokenExpirationKey].Equals("Sliding") ? TokenExpiration.Sliding : TokenExpiration.Absolute;
			}
			if (!string.IsNullOrEmpty(configuration[RedirectUrisKey]))
			{
				RedirectUris = configuration[RedirectUrisKey].Trim().Split(",");
			}
			if (!string.IsNullOrEmpty(configuration[PostLogoutRedirectUrisKey]))
			{
				PostLogoutRedirectUris = configuration[PostLogoutRedirectUrisKey].Trim().Split(",");
			}

			if (!string.IsNullOrEmpty(configuration[BackChannelLogoutKey]))
			{
				BackChannelLogoutUri = configuration[BackChannelLogoutKey].Trim();
			}

			if (!string.IsNullOrEmpty(configuration[AlwaysIncludeUserClaimsInIdTokenKey]))
			{
				AlwaysIncludeUserClaimsInIdToken = bool.Parse(configuration[AlwaysIncludeUserClaimsInIdTokenKey]);
			}

			if (!string.IsNullOrEmpty(configuration[RequireConsentKey]))
			{
				RequireConsent = bool.Parse(configuration[RequireConsentKey]);
			}
		}

		int StringToIntMultiplication(string multiplication)
		{
			int result = 1;
			foreach (var element in multiplication.Split("*"))
			{
				result *= int.Parse(element);
			}

			return result;
		}
	}
}
