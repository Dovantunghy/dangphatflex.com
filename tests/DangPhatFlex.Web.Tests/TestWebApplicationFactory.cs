using System;
using System.IO;
using System.Linq;
using DangPhatFlex.Web.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DangPhatFlex.Web.Tests;

/// <summary>
/// A <see cref="WebApplicationFactory{TEntryPoint}"/> that points <see cref="AppDbContext"/> at a
/// uniquely-named SQLite file per factory instance instead of the shared
/// "Data Source=dangphatflex.db" configured in appsettings.json.
///
/// Each xUnit test class gets its own <see cref="IClassFixture{TFixture}"/> instance of this
/// factory (and therefore its own database file), which removes two problems that arose from
/// every test class sharing one physical SQLite file under xUnit's default parallel
/// test-class execution:
///   1. Concurrent Database.Migrate()/DbSeeder.Seed()/IdentitySeeder.SeedAsync() calls racing
///      against each other on the same file (intermittent "UNIQUE constraint failed" failures).
///   2. Test classes silently observing each other's persisted data (e.g. a ContactSubmission
///      row written by one class being visible to another class's tests).
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"test-{Guid.NewGuid()}.db";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={_dbName}"));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing && File.Exists(_dbName))
        {
            try
            {
                File.Delete(_dbName);
            }
            catch
            {
                // Best-effort cleanup; the file may still be briefly locked by SQLite.
            }
        }
    }
}
