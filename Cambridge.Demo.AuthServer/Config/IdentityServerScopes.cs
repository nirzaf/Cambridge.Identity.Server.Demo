using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Cambridge.Demo.AuthServer.Config
{
	public static class IdentityServerScopes
	{
		public static IdentityResource OpenId = new IdentityResources.OpenId();
		public static IdentityResource Email = new IdentityResources.Email();
		public static IdentityResource Profile = new IdentityResources.Profile();
		public static IdentityResource Address = new IdentityResources.Address();
		public static IdentityResource Phone = new IdentityResources.Phone();
		public static ApiResource CambridgeDemo = new("cambridgedemoapi", "Cambridge Demo Api");
		public static ApiResource CompanyScope = new("companyscope","Company Scope", new []{"role"});
		
		public static List<IdentityResource> GetIdentityResources() => new()
		{
			OpenId,
			Email,
			Profile,
			Address,
			Phone
		};

		public static List<ApiResource> GetApiResources() => new()
		{
			CambridgeDemo,
			CompanyScope,
		};
	}
}