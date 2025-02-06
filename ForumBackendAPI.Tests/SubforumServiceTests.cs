using ForumBackendAPI.Controllers;
using ForumBackendAPI.Models;
using ForumBackendAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace ForumBackendAPITest;

[Collection("SharedDbCtxCollection")]
public class SubforumServiceTests(TestDatabaseFixture fixture) : IClassFixture<TestDatabaseFixture>
{
    private TestDatabaseFixture Fixture { get; }= fixture;

    [Fact]
    public async Task GetAll_ExistingCategory_ReturnSubforums()
    {
        await using var context = Fixture.CreateContext();
        var logger = Mock.Of<ILogger<SubforumService>>();
        var categoryService = Mock.Of<ICategoryService>();

        var subforumService = new SubforumService(logger, context, categoryService);

        var result = await subforumService.GetAll(1);

        var model = Assert.IsAssignableFrom<IList<Subforum>>(result);
        
        Assert.Equal(3, model.Count);
    }
    
    [Fact]
    public async Task GetAll_NonExistingCategory_ReturnNoSubforum()
    {
        await using var context = Fixture.CreateContext();
        var logger = Mock.Of<ILogger<SubforumService>>();
        var categoryService = Mock.Of<ICategoryService>();

        var subforumService = new SubforumService(logger, context, categoryService);

        var result = await subforumService.GetAll(-999);

        var model = Assert.IsAssignableFrom<IList<Subforum>>(result);
        
        Assert.Empty(model);
    }

    [Fact]
    public async Task Create_NonExistentCategory_ReturnFalse()
    {
        await using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var logger = Mock.Of<ILogger<SubforumService>>();
        var categoryService = Mock.Of<ICategoryService>();
        var subforumService = new SubforumService(logger, context, categoryService);
        var name = "TestName";
        var description = "TestDescription";

        context.ChangeTracker.Clear();
        
        var result = await subforumService.Create(name, description, -999);
        
        Assert.False(result);
    }
    
    [Fact]
    public async Task Create_ExistentCategory_ReturnTrue()
    { 
        await using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var logger = Mock.Of<ILogger<SubforumService>>();
        var categoryService = new Mock<ICategoryService>();
        categoryService.Setup(c => c.GetCategoryForId(1))
            .ReturnsAsync(new Category());
        var subforumService = new SubforumService(logger, context, categoryService.Object);
        var name = "TestName";
        var description = "TestDescription";

        var result = await subforumService.Create(name, description, 1);

        context.ChangeTracker.Clear();
        
        Assert.True(result);
    }

    [Fact]
    public async Task Update_ExistingSubforumChangedName_ReturnOneRowChanged()
    {
        await using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var logger = Mock.Of<ILogger<SubforumService>>();
        var categoryService = new Mock<ICategoryService>();

        var subforumService = new SubforumService(logger, context, categoryService.Object);

        var updatedName = "Subforum1UpdatedName";

        var rowsUpdated = await subforumService.Update(1, name: updatedName);
        
        Assert.Equal(1, rowsUpdated);

        var updatedSubforum = await context.Subforums
            .Where(s => s.SubforumId == 1)
            .ToListAsync();
        
        // name has been changed
        Assert.Equal(updatedName, updatedSubforum[0].Name);
        
        // did the description stay the same?
        Assert.Equal("TestSubforum1Description", updatedSubforum[0].Description);
    }
    
    [Fact]
    public async Task Update_ExistingSubforumChangedDescription_ReturnOneRowChanged()
    {
        await using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var logger = Mock.Of<ILogger<SubforumService>>();
        var categoryService = new Mock<ICategoryService>();

        var subforumService = new SubforumService(logger, context, categoryService.Object);

        var updatedDescription = "Subforum1UpdatedDescription";

        var rowsUpdated = await subforumService.Update(1, description: updatedDescription);
        
        Assert.Equal(1, rowsUpdated);

        var updatedSubforum = await context.Subforums
            .Where(s => s.SubforumId == 1)
            .ToListAsync();
        
        // description has been changed
        Assert.Equal(updatedDescription, updatedSubforum[0].Description);
        
        // did the name stay the same?
        Assert.Equal("TestSubforum1Cat1", updatedSubforum[0].Name);
    }

    [Fact]
    public async Task Delete_ExistingFKToCategory_ReturnTrue()
    {
        await using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var logger = Mock.Of<ILogger<SubforumService>>();
        var categoryService = new Mock<ICategoryService>();

        var subforumService = new SubforumService(logger, context, categoryService.Object);

        // delete subforum with id 1 (which is part of category with id 1 as well)
        var result = await subforumService.Delete(1);
        Assert.True(result);

        var after = await subforumService.GetAll(1);
        
        context.ChangeTracker.Clear();
        
        var model = Assert.IsAssignableFrom<IList<Subforum>>(after);
        
        // there should be 2 subforums left.
        Assert.Equal(2, model.Count);
    }
    
    [Fact]
    public async Task Delete_NonExistingFKToCategory_ReturnFalse()
    {
        await using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var logger = Mock.Of<ILogger<SubforumService>>();
        var categoryService = new Mock<ICategoryService>();

        var subforumService = new SubforumService(logger, context, categoryService.Object);

        // delete subforum with id -999 (doesn't exist)
        var result = await subforumService.Delete(-999);
        Assert.False(result);
    }
}