using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Trakmark.Data;

/// <summary>
/// Design-time factory that allows <c>dotnet ef</c> to scaffold migrations
/// without a running application or live connection string.
/// Reads the connection string from the <c>TRAKMARK_DESIGN_TIME_CONNSTR</c>
/// environment variable, falling back to a LocalDB default when the variable
/// is not set.
/// </summary>
internal sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    /// <summary>
    /// Creates a configured <see cref="ApplicationDbContext"/> for design-time
    /// tooling such as <c>dotnet ef migrations add</c>.
    /// </summary>
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("TRAKMARK_DESIGN_TIME_CONNSTR")
            ?? @"Server=(localdb)\mssqllocaldb;Database=Trakmark;Trusted_Connection=True;";

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new ApplicationDbContext(options);
    }
}
