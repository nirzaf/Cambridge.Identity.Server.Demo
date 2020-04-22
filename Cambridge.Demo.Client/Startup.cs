using Cambridge.Demo.Client.Config;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Cambridge.Demo.Client
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
			ConfigureAuthentication(services);
			services.AddControllersWithViews();
		}

		void ConfigureAuthentication(IServiceCollection services)
		{
			var identitySettings = new IdentitySettings(Configuration);

			services.AddSingleton(identitySettings);
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

			//Set Up Authentication Handlers and Services
			services.AddAuthentication(options =>
			{
				//Request User Authentication using OpenId Connect
				options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
				options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			})
			.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
			options =>
			{
				options.LoginPath = new PathString(identitySettings.LoginPath);
			})
			.AddOpenIdConnect( // By default OpenIdConnectDefaults.AuthenticationScheme
			options =>
			{
	 			options.Authority = identitySettings.Issuer;
				options.SaveTokens = true;
				options.ClientId = identitySettings.ClientId;
				options.ClientSecret = identitySettings.ClientSecret;
				options.ResponseType = identitySettings.ResponseType;
				options.RequireHttpsMetadata = true;
				options.GetClaimsFromUserInfoEndpoint = true;
				options.TokenValidationParameters.NameClaimType = "given_name";
				options.TokenValidationParameters.RoleClaimType = "role";
				options.ClaimActions.DeleteClaim("s_hash");
				var scopes = identitySettings.Scopes.Split(',');
				foreach (var scope in scopes)
				{
					options.Scope.Add(scope);
				}
				options.Events = new OpenIdConnectEvents()
				{
					OnRemoteFailure = context =>
					{
						if (context.Failure.Data["error"].Equals("access_denied"))
						{
							context.Response.Redirect("/Account/AccessDenied?error=Access_Denied");
							context.HandleResponse();
						}
						return Task.FromResult(0);
					}
				};
			});


			services.AddHttpClient("apiClient", options =>
			{
				options.BaseAddress = new Uri(Configuration["ApiUrl"]);
			});
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

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
