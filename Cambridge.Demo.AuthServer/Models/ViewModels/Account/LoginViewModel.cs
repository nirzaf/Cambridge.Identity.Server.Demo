using System.ComponentModel.DataAnnotations;

namespace Cambridge.Demo.AuthServer.Models
{
	public class LoginViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		public string ReturnUrl { get; set; }
	}
}
