using NSubstitute;
using SmartBar.Application.Common.Interfaces;
using SmartBar.Infrastructure.Data;


namespace SmartBar.Application.IntegrationTests.Infrastructure;

/// <summary>
/// Base class for integration tests. Creates a fresh in-memory SQLite database
/// per test to ensure full isolation between tests.
/// </summary>
public abstract class IntegrationTestBase : IDisposable
{
    protected readonly TestDatabase Database;
    protected readonly ApplicationDbContext Context;
    protected readonly IUser User;

    protected IntegrationTestBase()
    {
        Database = TestDatabase.CreateFresh();
        Context = Database.CreateContext();

        User = Substitute.For<IUser>();
        User.Id.Returns(Guid.NewGuid().ToString());
    }

    public void Dispose()
    {
        Context.Dispose();
        Database.Dispose();
    }
}
