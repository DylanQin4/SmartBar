using NSubstitute;
using SmartBar.Application.Common.Interfaces;

namespace SmartBar.Application.Tests.Common.Fixtures;

public static class MockUserFactory
{
    public static IUser Create(string? id = null)
    {
        var user = Substitute.For<IUser>();
        user.Id.Returns(id ?? Guid.NewGuid().ToString());
        user.Roles.Returns(new List<string> { "Administrator" });
        return user;
    }
}
