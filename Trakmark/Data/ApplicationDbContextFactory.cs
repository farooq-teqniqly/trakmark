using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Trakmark.Data;

/// <summary>
/// Design-time factory that allows <c>dotnet ef</c> to scaffold migrations
/// without a running application or live connection string.
/// </summary>
internal sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    /// <inheritdoc/>
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=TrakmarkDesignTime;Trusted_Connection=True;")
            .Options;

        return new ApplicationDbContext(options);
    }
}
