﻿

using System.Reflection;
using Application.Services;
using Application.Services.Identity;
using Infrastructure.Context;
using Infrastructure.Services;
using Infrastructure.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContext<ApplicationDbContext>(options => options
            //        .UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
            //        .AddTransient<ApplicationDbSeeder>();

            services.AddDbContext<ApplicationDbContext>(options => options
                    .UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                .AddTransient<ApplicationDbSeeder>();
            return services;

        }

        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services
                .AddTransient<ITokenService, TokenService>()
                .AddTransient<IUserService, UserService>()
                .AddTransient<IRoleService, RoleService>()
                .AddHttpContextAccessor()
                .AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddTransient<IEmployeeService, EmployeeService>();

            return services;
        }

        public static void AddInfrastrustureDependencies(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }
    }
}
