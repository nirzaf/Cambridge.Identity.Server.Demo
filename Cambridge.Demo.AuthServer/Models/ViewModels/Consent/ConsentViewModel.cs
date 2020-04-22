using System.Collections.Generic;

namespace Cambridge.Demo.AuthServer.Models
{
	public class ConsentViewModel : ConsentInputModel
	{
		public string ClientName { get; set; }
		public string ClientUrl { get; set; }
		public bool AllowRememberConsent { get; set; }

		public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }
		public IEnumerable<ScopeViewModel> ApiScopes { get; set; }
    }
}