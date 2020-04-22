using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Cambridge.Demo.AuthServer.Config
{
	public class ClientSettings
	{
		const string ClientSectionKey = "ClientSettings:Client";

		public ClientSettings(IConfiguration configuration)
		{
			Clients = new List<ClientSetting>();
			for (int i = 1; configuration.GetSection(ClientSectionKey + i).Exists(); i++)
			{
				Clients.Add(new ClientSetting(configuration.GetSection(ClientSectionKey + i)));
			}
		}

		public List<ClientSetting> Clients { get; }
	}
}