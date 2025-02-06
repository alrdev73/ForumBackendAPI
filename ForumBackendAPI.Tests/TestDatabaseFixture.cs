using ForumBackendAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ForumBackendAPITest;

public class TestDatabaseFixture
{

    public TestDatabaseFixture()
    {
        using var context = CreateContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        Cleanup();
    }

    public ForumContext CreateContext()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        return new ForumContext(
                    new DbContextOptionsBuilder<ForumContext>()
                        .UseNpgsql(config.GetConnectionString("Test"))
                        .Options, config);
    }

    public void Cleanup()
    {
        using var context = CreateContext();
        
        context.Categories.RemoveRange(context.Categories);
        context.Subforums.RemoveRange(context.Subforums);
        context.Threads.RemoveRange(context.Threads);
        
        context.AddRange(
            new Category{Name = "TestCategory1"},
            new Category{Name = "TestCategory2"},
            new Subforum{CategoryId = 1, Name = "TestSubforum1Cat1", Description = "TestSubforum1Description"},
            new Subforum{CategoryId = 1, Name = "TestSubforum2Cat1"},
            new Subforum{CategoryId = 1, Name = "TestSubforum3Cat1"},
            new Subforum{CategoryId = 2, Name = "TestSubforum1Cat2"},
            new Subforum{CategoryId = 2, Name = "TestSubforum2Cat2"},
            new ForumThread{SubforumId = 1, Name = "Subforum1Thread1"},
            new ForumThread{SubforumId = 1, Name = "Subforum1Thread2"},
            new ForumThread{SubforumId = 1, Name = "Subforum1Thread3"}
            );
        context.SaveChanges();
    }
}