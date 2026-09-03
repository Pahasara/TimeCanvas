using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TimeCanvas.Core.Data;

public class TimeCanvasDbContextFactory : IDesignTimeDbContextFactory<TimeCanvasDbContext>
{
    public TimeCanvasDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TimeCanvasDbContext>();
        optionsBuilder.UseSqlite($"Data Source={AppPaths.DatabaseFile}");
        return new TimeCanvasDbContext(optionsBuilder.Options);
    }
}
