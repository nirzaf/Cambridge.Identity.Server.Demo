using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cambridge.Demo.ResourceServer.Policies
{
	public class DocumentOwnerAuthorizationHandler : AuthorizationHandler<DocumentOwnerRequirement, Document>
	{
		/// <summary>
		/// Check if the User associated with the Access Token has right Access to access the Specified Document
		/// </summary>
		/// <returns></returns>
		protected override  Task HandleRequirementAsync(
			AuthorizationHandlerContext context, 
			DocumentOwnerRequirement requirement,
			Document document)
		{
			var user = context.User.Claims.SingleOrDefault(c => c.Type.Equals("sub"))?.Value;
			
			if (document.Owner.Equals(Guid.Parse(user)))
				context.Succeed(requirement);

			return Task.FromResult(0);
		}
	}
}
