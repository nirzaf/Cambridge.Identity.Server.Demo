using System;
using Cambridge.Demo.AuthServer.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cambridge.Demo.AuthServer
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{ 
			ClientSettings clientSettings = new ClientSettings(Configuration);

			services.AddControllersWithViews();
			services.AddRazorPages();
			//Add Identity Server service
			services.AddIdentityServer(options =>
				{
					//Set The login Page
					options.UserInteraction.LoginUrl = "/Account/Login";
					//For Logging
					options.Events.RaiseErrorEvents = true;
					options.Events.RaiseFailureEvents = true;
					options.Events.RaiseInformationEvents = true;
					options.Events.RaiseSuccessEvents = true;
				})
				//Add Dev configurations
				.AddTestUsers(UserConfig.GetUsers())
				.AddDeveloperSigningCredential()
				.AddInMemoryClients(clientSettings.Clients)
				.AddInMemoryApiResources(
					IdentityServerScopes.GetApiResources()
				)
				.AddInMemoryIdentityResources(
					IdentityServerScopes.GetIdentityResources()
				);
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();

			app.Use(async (ctx, next) =>
			{
				Console.WriteLine(ctx.Request.Path);
				await next();
			});

			app.UseIdentityServer();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");

				endpoints.MapRazorPages();
			});
		}
	}
}
