using ForumBackendAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ForumBackendAPITest;

public class TestDatabaseFixture
{

    private static readonly object _lock = new();
    private static bool _databaseInitialised;

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialised)
            {
                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    context.AddRange(
                        new Category{Name = "TestCategory1"},
                        new Category{Name = "TestCategory2"});
                    context.SaveChanges();
                }
            }
        }
    }

    public ForumContext CreateContext()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        return new ForumContext(
                    new DbContextOptionsBuilder<ForumContext>()
                        .UseNpgsql(config.GetConnectionString("Default"))
                        .Options, config);
    }
        
}