using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Cambridge.Demo.Client.Models
{
	public class TokenViewModel
	{
		public string Header { get; set; }
		public string Body { get; set; }
		public string Signature { get; set; }
		public JObject PayloadData { get; set; }
	}
}
