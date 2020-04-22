using Microsoft.AspNetCore.Authorization;

namespace Cambridge.Demo.ResourceServer.Policies
{
	public class DocumentOwnerRequirement : IAuthorizationRequirement
 	{
    }

	public static class DocumentOwnerPolicyExtension
	{
		public static AuthorizationPolicyBuilder RequireDocumentOwner(this AuthorizationPolicyBuilder builder)
		{
			builder.AddRequirements(new DocumentOwnerRequirement());
			return builder;
		}
	}
}
