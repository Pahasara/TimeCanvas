using TimeCanvas.Core.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace TimeCanvas.Core.Tests;

public class TestDbContextFactory : IDbContextFactory<TimeCanvasDbContext>, IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<TimeCanvasDbContext> _options;

    public TestDbContextFactory()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<TimeCanvasDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var db = new TimeCanvasDbContext(_options);
        db.Database.EnsureCreated();
    }

    public TimeCanvasDbContext CreateDbContext() => new(_options);

    public Task<TimeCanvasDbContext> CreateDbContextAsync(CancellationToken ct = default) =>
        Task.FromResult(CreateDbContext());

    public void Dispose() => _connection.Dispose();
}
