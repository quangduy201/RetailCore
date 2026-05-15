using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RetailCore.Repositories.Data;

namespace RetailCore.IntegrationTests.Fixtures;

public sealed class SqliteTestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    public SqliteTestDatabase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        Options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = CreateContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    public DbContextOptions<AppDbContext> Options { get; }

    public AppDbContext CreateContext()
    {
        return new AppDbContext(Options);
    }

    public async Task ResetAsync()
    {
        await using var context = CreateContext();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
