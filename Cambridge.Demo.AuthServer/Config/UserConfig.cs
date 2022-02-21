using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace Cambridge.Demo.AuthServer.Config
{
	public static class UserConfig
	{
		public static List<TestUser> GetUsers()
		{
			return new List<TestUser>
			{
				new()
                {
					SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
					Username = "andrea.angella@cambridgedemo.com",
					Password = "password",

					Claims = new List<Claim>
					{
						new("given_name", "Andrea"),
						new("family_name", "Angella"),
						new("address", "1, Main Road"),
						new("birthdate","14/07/1960"),
						new("phone_number","01236"),
						new("email","andrea.angella@cambridgedemo.com"),
						new("role","Software Engineer")
					}
				},

				new()
                {
					SubjectId = "24f56fef-5cfc-44e0-b77a-8bae834ed030",
					Username = "stefano.donofrio@cambridgedemo.com",
					Password = "password",

					Claims = new List<Claim>
					{
						new("given_name", "Stefano"),
						new("family_name", "D'Onofrio"),
						new("address", "2, Big Street"),
						new("birthdate","19/09/2010"),
						new("phone_number","321321"),
						new("email","stefano.donofrio@cambridgedemo.com"),
						new("role","Manager")
					}
				}
			};
		}
	}
}
