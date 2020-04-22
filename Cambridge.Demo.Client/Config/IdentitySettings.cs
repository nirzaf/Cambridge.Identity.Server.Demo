using Microsoft.Extensions.Configuration;

namespace Cambridge.Demo.Client.Config
{
	public class IdentitySettings
	{
		const string IssuerKey = "IdentitySetting:Issuer";
		const string ClientIdKey = "IdentitySetting:ClientId";
		const string ClientSecretKey = "IdentitySetting:ClientSecret";
		const string ScopesKey = "IdentitySetting:Scopes";
		const string ResponseTypeKey = "IdentitySetting:ResponseType";
		const string TokenEndpointKey = "IdentitySetting:TokenEndpoint";
		const string LoginPathKey = "IdentitySetting:LoginPath";
		const string RevocationEndpointKey = "IdentitySetting:RevocationEndpoint";

		public IdentitySettings(IConfiguration configuration)
		{
			Issuer = configuration[IssuerKey];
			ClientId = configuration[ClientIdKey];
			ClientSecret = configuration[ClientSecretKey];
			Scopes = configuration[ScopesKey];
			ResponseType = configuration[ResponseTypeKey];
			TokenEndpoint = Issuer + configuration[TokenEndpointKey];
			RevocationEndpoint = Issuer + configuration[RevocationEndpointKey];
			LoginPath = configuration[LoginPathKey];
		}

		public string Issuer { get; set; }
		public string ClientId { get; }
		public string ClientSecret { get; }
		public string Scopes { get; }
		public string ResponseType { get; }
		public string TokenEndpoint { get; }
		public string RevocationEndpoint { get; }
		public string LoginPath { get; }
	}
}
