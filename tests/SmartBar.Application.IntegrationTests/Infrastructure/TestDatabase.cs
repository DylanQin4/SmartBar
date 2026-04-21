using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SmartBar.Application.Common.Interfaces;
using SmartBar.Infrastructure.Data;

namespace SmartBar.Application.IntegrationTests.Infrastructure;

/// <summary>
/// Manages a shared SQLite in-memory database for integration tests.
/// Uses a persistent connection so the DB survives between operations.
/// Applies all EF Core configurations from Infrastructure (OwnsOne, HasMany, etc.).
/// </summary>
public class TestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public TestDatabase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = CreateContext();
        context.Database.EnsureCreated();
    }

    public ApplicationDbContext CreateContext()
    {
        return new ApplicationDbContext(_options);
    }

    public IApplicationDbContext CreateDbContext() => CreateContext();

    /// <summary>
    /// Creates a standalone TestDatabase for a single test.
    /// Use this when tests need full isolation.
    /// </summary>
    public static TestDatabase CreateFresh() => new();

    public void Dispose()
    {
        _connection.Dispose();
    }
}
