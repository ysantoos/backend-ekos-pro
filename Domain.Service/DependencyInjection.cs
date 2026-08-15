using Domain.Service.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Domain.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddDomainService(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Register MediatR
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);

            // Register validation behavior pipeline
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
