using Microsoft.Extensions.Logging;
using SmartBar.Application.Common.Interfaces;

namespace SmartBar.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest, TResponse>(
    ILogger<TRequest> logger,
    IUser user,
    IIdentityService identityService) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = user.Id ?? string.Empty;
        var userName = string.Empty;

        if (!string.IsNullOrEmpty(userId))
        {
            userName = await identityService.GetUserNameAsync(userId) ?? string.Empty;
        }

        logger.LogInformation("Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);

        return await next(cancellationToken);
    }
}
