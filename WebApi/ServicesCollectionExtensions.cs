using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebApi.Permissions;

namespace WebApi
{
    public static class ServicesCollectionExtensions
    {
        internal static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
        {
	        using var serviceScope = app.ApplicationServices.CreateScope();
			var seeders = serviceScope.ServiceProvider.GetServices<ApplicationDbSeeder>();

			foreach (var seeder in seeders)
			{
				seeder.SeedDatabaseAsync().GetAwaiter().GetResult();
			}

			return app;
		}

		internal static IServiceCollection AddIdentitySettings(this IServiceCollection services)
		{
			services
				.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
				.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>()
				.AddIdentity<ApplicationUser, ApplicationRole>(options =>
				{
					options.User.RequireUniqueEmail = true;
					options.Password.RequireDigit = false;
					options.Password.RequireLowercase = false;
					options.Password.RequireUppercase = false;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequiredLength = 6;
				})
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			return services;
		}
	}
}
