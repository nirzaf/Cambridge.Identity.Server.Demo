using System;
using Cambridge.Demo.ResourceServer.Policies;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cambridge.Demo.ResourceServer
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
			services.AddControllers();

			//Register IdentityServer Authentication Handler
			//Whilst AddJwtBearerToken could be used ( and have a better name), this handler have additional benefit ( reference token can be used, Scope Validation.. )
			services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
				.AddIdentityServerAuthentication(options =>
				{
					//Authorization Server Address
					options.Authority = Configuration["ApiSetting:Authority"];
					options.RequireHttpsMetadata = true;
					options.JwtValidationClockSkew = TimeSpan.Zero;
					options.CacheDuration = TimeSpan.Zero;
					//An Audience of ApiName must be present in the Access token 
					options.ApiName = Configuration["ApiSetting:ApiName"];
				});

			//Add Two different Authorization Policies
			services.AddAuthorization(options =>
			{
				//The Access Token must have companyscope in the Audience
				options.AddPolicy("ScopedPolicy", policy =>
				{
					policy.RequireScope("companyscope");
				});
				//Custom Authorization Handler to implement the Resource-Based Policy
				options.AddPolicy("DocumentOwnerPolicy", policy => { policy.RequireDocumentOwner(); });
			});

			services.AddSingleton<IAuthorizationHandler, DocumentOwnerAuthorizationHandler>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
