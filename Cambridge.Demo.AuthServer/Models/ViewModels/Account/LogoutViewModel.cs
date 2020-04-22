namespace Cambridge.Demo.AuthServer.Models
{
	public class LogoutViewModel
	{
		public string PostLogoutRedirectUri { get; set; }
		public string SignOutIframeUrl { get; set; }
		public string LogoutId { get; set; }
    }
}