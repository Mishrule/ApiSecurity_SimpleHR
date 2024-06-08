﻿using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Application.AppConfigs;
using Common.Authorization;
using Common.Responses.Wrappers;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
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

		public static IServiceCollection AddJwtSettings(this IServiceCollection services, AppConfiguration config)
		{
			var key = Encoding.ASCII.GetBytes(config.Secret);
			services
				.AddAuthentication(authentication =>
				{
					authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(bearer =>
				{
					bearer.RequireHttpsMetadata = false;
					bearer.SaveToken = true;
					bearer.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(key),
						ValidateIssuer = false,
						ValidateAudience = false,
						RoleClaimType = ClaimTypes.Role,
						ClockSkew = TimeSpan.Zero
					};

					bearer.Events = new JwtBearerEvents
					{
						OnAuthenticationFailed = c =>
						{
							if (c.Exception is SecurityTokenExpiredException)
							{
								c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
								c.Response.ContentType = "application/json";
								var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("Token has expired"));
								return c.Response.WriteAsync(result);
							}
							else
							{
								c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
								c.Response.ContentType = "application/json";
								var result = JsonConvert.SerializeObject(
									ResponseWrapper.Fail($"An unhandled error has occured | {c.Exception.Message}"));
								return c.Response.WriteAsync(result);
							}
						},
						OnChallenge = context =>
						{
							context.HandleResponse();
							if (!context.Response.HasStarted)
							{
								context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
								context.Response.ContentType = "application/json";
								var result =
									JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized"));
								context.Response.WriteAsync(result);
							}

							return Task.CompletedTask;
						},
						OnForbidden = context =>
						{
							context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
							context.Response.ContentType = "application/json";
							var result =
								JsonConvert.SerializeObject(
									ResponseWrapper.Fail("You are not authorized to access this resource"));
							return context.Response.WriteAsync(result);
						},
					};
				});
			services.AddAuthorization(options =>
			{
				foreach (var prop in typeof(AppPermissions).GetNestedTypes().SelectMany(c =>
					         c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
				{
					var propertyValue = prop.GetValue(null);
					if (propertyValue is not null)
					{
						options.AddPolicy(propertyValue.ToString(),
							policy => policy.RequireClaim(AppClaim.Permission, propertyValue.ToString()));
					}
				}
			});
			return services;
		}
	}
}
