using System.Security.Claims;
using SmartBar.Application.Common.Interfaces;

namespace SmartBar.API.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    public string? Id => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public List<string>? Roles => httpContextAccessor.HttpContext?.User?
        .FindAll(ClaimTypes.Role)
        .Select(c => c.Value)
        .ToList();
}
