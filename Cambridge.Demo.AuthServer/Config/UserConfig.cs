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
				new TestUser
				{
					SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
					Username = "andrea.angella@cambridgedemo.com",
					Password = "password",

					Claims = new List<Claim>
					{
						new Claim("given_name", "Andrea"),
						new Claim("family_name", "Angella"),
						new Claim("address", "1, Main Road"),
						new Claim("birthdate","14/07/1960"),
						new Claim("phone_number","01236"),
						new Claim("email","andrea.angella@cambridgedemo.com"),
						new Claim("role","Software Engineer")
					}
				},

				new TestUser
				{
					SubjectId = "24f56fef-5cfc-44e0-b77a-8bae834ed030",
					Username = "stefano.donofrio@cambridgedemo.com",
					Password = "password",

					Claims = new List<Claim>
					{
						new Claim("given_name", "Stefano"),
						new Claim("family_name", "D'Onofrio"),
						new Claim("address", "2, Big Street"),
						new Claim("birthdate","19/09/2010"),
						new Claim("phone_number","321321"),
						new Claim("email","stefano.donofrio@cambridgedemo.com"),
						new Claim("role","Manager")
					}
				}
			};
		}
	}
}
