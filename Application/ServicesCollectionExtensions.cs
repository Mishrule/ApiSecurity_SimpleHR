using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.Pipelines;
using FluentValidation;

namespace Application
{
	public static class ServicesCollectionExtensions
	{
		public static IServiceCollection AddApplicationService(this IServiceCollection services)
		{
			//services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
			var assembly = Assembly.GetExecutingAssembly();

			return services
					.AddMediatR(assembly)
					.AddAutoMapper(assembly)
					.AddValidatorsFromAssembly(assembly)
					.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>))
				;
		}
    }
}
