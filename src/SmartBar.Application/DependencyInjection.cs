using System.Reflection;
using SmartBar.Application.Common.Behaviours;
using SmartBar.Application.Common.Mediator;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Mediator
        services.AddScoped<ISender, SmartBar.Application.Common.Mediator.Mediator>();

        // Behaviours
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

        // Scan handlers: IRequestHandler<TRequest> and IRequestHandler<TRequest, TResponse>
        services.AddHandlersFromAssembly(assembly);

        // Scan validators: AbstractValidator<T>
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }

    private static void AddHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType &&
                    (i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                     i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
                .Select(i => (ServiceType: i, ImplementationType: t)));

        foreach (var (serviceType, implementationType) in handlerTypes)
        {
            services.AddTransient(serviceType, implementationType);
        }
    }

    private static void AddValidatorsFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var validatorTypes = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IValidator<>))
                .Select(i => (ServiceType: i, ImplementationType: t)));

        foreach (var (serviceType, implementationType) in validatorTypes)
        {
            services.AddScoped(serviceType, implementationType);
        }
    }
}
