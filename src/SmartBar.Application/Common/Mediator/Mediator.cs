using Microsoft.Extensions.DependencyInjection;

namespace SmartBar.Application.Common.Mediator;

public class Mediator(IServiceProvider serviceProvider) : ISender
{
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handler = serviceProvider.GetRequiredService(handlerType);

        var behaviors = serviceProvider
            .GetServices<IPipelineBehavior<IRequest<TResponse>, TResponse>>()
            .Reverse()
            .ToList();

        // Build the pipeline: behaviors wrap the handler
        RequestHandlerDelegate<TResponse> pipeline = (ct) =>
        {
            var method = handlerType.GetMethod("Handle")!;
            return (Task<TResponse>)method.Invoke(handler, [request, ct])!;
        };

        foreach (var behavior in behaviors)
        {
            var current = pipeline;
            pipeline = (ct) => behavior.Handle(request, current, ct);
        }

        return pipeline(cancellationToken);
    }

    public Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
        var handler = serviceProvider.GetRequiredService(handlerType);

        // For void requests, wrap in Unit pattern to reuse pipeline
        var method = handlerType.GetMethod("Handle")!;
        return (Task)method.Invoke(handler, [request, cancellationToken])!;
    }
}
