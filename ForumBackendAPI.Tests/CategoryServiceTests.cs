using ForumBackendAPI.Models;
using ForumBackendAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ILogger = Castle.Core.Logging.ILogger;

namespace ForumBackendAPITest;

[Collection("SharedDbCtxCollection")]
public class CategoryServiceTests(TestDatabaseFixture fixture) : IClassFixture<TestDatabaseFixture>
{
    private TestDatabaseFixture Fixture { get; } = fixture;

    
    [Fact]
    public async Task Get_ReturnsTwoCategories()
    {
        await using var context = Fixture.CreateContext();
        var logger = Mock.Of<ILogger<CategoryService>>();
        var service = new CategoryService(logger, context);

        var result = await service.GetAll();

        var model = Assert.IsAssignableFrom<IEnumerable<Category>>(result);
        
        Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task Create_ValidCategory_AddedToDatabase()
    {
        await using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var logger = Mock.Of<ILogger<CategoryService>>();
        var service = new CategoryService(logger, context);
        var category = new Category
        {
            CategoryId = 999,
            Name = "TestCategory3"
        };

        await service.Create(category);
        
        context.ChangeTracker.Clear();

        var blog = await context.Categories.SingleAsync(c => c.CategoryId == 999);
        Assert.Equal("TestCategory3", blog.Name);
    }

    [Fact]
    public async Task Rename_ExistentCategoryValidNewName_UpdatesName()
    {
        await using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var logger = Mock.Of<ILogger<CategoryService>>();
        var service = new CategoryService(logger, context);

        var rowsUpdated = await service.Rename(1, "NewName");
        context.ChangeTracker.Clear();
        
        Assert.Equal(1, rowsUpdated);
    }

    [Fact]
    public async Task Rename_ExistentCategory_EmptyName_UpdatesNoRows()
    {
        await using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var logger = Mock.Of<ILogger<CategoryService>>();
        var service = new CategoryService(logger, context);

        var rowsUpdated = await service.Rename(1, string.Empty);
        context.ChangeTracker.Clear();
        
        Assert.Equal(0, rowsUpdated);
    }
    
    [Fact]
    public async Task Rename_NonExistentCategory_UpdatesNoRows()
    {
        await using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var logger = Mock.Of<ILogger<CategoryService>>();
        var service = new CategoryService(logger, context);

        var rowsUpdated = await service.Rename(-999, "name");
        context.ChangeTracker.Clear();
        
        Assert.Equal(0, rowsUpdated);
    }

    [Fact]
    public async Task Delete_ExistentCategoryId_ReturnsTrue()
    {
        await using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var logger = Mock.Of<ILogger<CategoryService>>();
        var service = new CategoryService(logger, context);

        var deleted = await service.Delete(1);
        context.ChangeTracker.Clear();
        
        Assert.True(deleted);
    }
    
    [Fact]
    public async Task Delete_NonExistentCategoryId_ReturnsFalse()
    {
        await using var context = Fixture.CreateContext();
        
        await context.Database.BeginTransactionAsync();
        var logger = Mock.Of<ILogger<CategoryService>>();
        var service = new CategoryService(logger, context);

        var deleted = await service.Delete(999);
        context.ChangeTracker.Clear();
        
        Assert.False(deleted);
    }
}