using ForumBackendAPI.Models;
using ForumBackendAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace ForumBackendAPITest;

[Collection("SharedDbCtxCollection")]
public class ForumThreadServiceTests(TestDatabaseFixture fixture) : IClassFixture<TestDatabaseFixture>
{
    private TestDatabaseFixture Fixture { get; } = fixture;

    [Fact]
    public async Task GetAll_ExistingSubforum_ReturnThreads()
    {
        await using var context = Fixture.CreateContext();
        var logger = Mock.Of<ILogger<ForumThreadService>>();
        var subforumService = Mock.Of<ISubforumService>();

        var threadService = new ForumThreadService(logger, context, subforumService);

        var result = await threadService.GetAll(1);

        var model = Assert.IsAssignableFrom<IList<ForumThread>>(result);
        
        Assert.Equal(3, model.Count);
    }

    [Fact]
    public async Task GetAll_NonExistingSubforum_ReturnEmptyList()
    {
        await using var context = Fixture.CreateContext();
        var logger = Mock.Of<ILogger<ForumThreadService>>();
        var subforumService = Mock.Of<ISubforumService>();

        var threadService = new ForumThreadService(logger, context, subforumService);

        var result = await threadService.GetAll(-999);

        var model = Assert.IsAssignableFrom<IList<ForumThread>>(result);
        
        Assert.Empty(model);
    }

    [Fact]
    public async Task Create_ExistingSubforum_ReturnTrue()
    {
        await using var context = Fixture.CreateContext();
        var logger = Mock.Of<ILogger<ForumThreadService>>();
        var subforumService = new Mock<ISubforumService>();

        await context.Database.BeginTransactionAsync();
        
        var name = "NewThread";
        var description = "NewThreadDesc";
        var author = "NewThreadAuthor";
        var subforumId = 1;
        
        var threadService = new ForumThreadService(logger, context, subforumService.Object);

        // for proper navigation when the thread entity is created
        var subforum = await context.Subforums.FindAsync(subforumId);
        subforumService.Setup(s => s.GetSubforumForId(subforumId))
            .ReturnsAsync(subforum);
        
        var oldThreadList = await context.Threads
            .Where(t => t.SubforumId == subforumId)
            .ToListAsync();
        
        var success = await threadService.Create(name, description, author, subforumId);
        Assert.True(success);

        var newThreadList = await context.Threads
            .Where(t => t.SubforumId == subforumId)
            .ToListAsync();
        
        Assert.Equal(oldThreadList.Count + 1, newThreadList.Count);
        
        context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task Create_NonExistingSubforum_ReturnFalse()
    {
        await using var context = Fixture.CreateContext();
        var logger = Mock.Of<ILogger<ForumThreadService>>();
        var subforumService = new Mock<ISubforumService>();

        await context.Database.BeginTransactionAsync();
        
        var name = "NewThread";
        var description = "NewThreadDesc";
        var author = "NewThreadAuthor";
        var subforumId = -999;
        
        var threadService = new ForumThreadService(logger, context, subforumService.Object);

        // subforum will be null since that id does not exist
        var subforum = await context.Subforums.FindAsync(subforumId);
        subforumService.Setup(s => s.GetSubforumForId(subforumId))
            .ReturnsAsync(subforum);
        
        var success = await threadService.Create(name, description, author, subforumId);
        Assert.False(success);
        
        context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task Update_NameForExistingThread_RowUpdatedAndThreadUpdated()
    {
        await using var context = Fixture.CreateContext();
        var logger = Mock.Of<ILogger<ForumThreadService>>();
        var subforumService = new Mock<ISubforumService>();
        
        await context.Database.BeginTransactionAsync();
        
        var name = "NewThreadName";
        var forumThreadId = 1;

        var threadService = new ForumThreadService(logger, context, subforumService.Object);

        var rowsUpdated = await threadService.Update(forumThreadId, name:name);
        
        Assert.Equal(1, rowsUpdated);

        var updatedThread = await context.Threads.FindAsync(forumThreadId);
        
        Assert.NotNull(updatedThread);
        Assert.Equal(name, updatedThread.Name);
        
        context.ChangeTracker.Clear();
    }
    
    [Fact]
    public async Task Update_DescriptionForExistingThread_RowUpdatedAndThreadUpdated()
    {
        await using var context = Fixture.CreateContext();
        var logger = Mock.Of<ILogger<ForumThreadService>>();
        var subforumService = new Mock<ISubforumService>();
        
        await context.Database.BeginTransactionAsync();
        
        var description = "NewThreadDescription";
        var forumThreadId = 1;

        var threadService = new ForumThreadService(logger, context, subforumService.Object);

        var rowsUpdated = await threadService.Update(forumThreadId, description:description);
        
        Assert.Equal(1, rowsUpdated);

        var updatedThread = await context.Threads.FindAsync(forumThreadId);
        
        Assert.NotNull(updatedThread);
        Assert.Equal(description, updatedThread.Description);
        
        context.ChangeTracker.Clear();
    }
    
    [Fact]
    public async Task Update_AnythingForNonExistingThread_ZeroRowsUpdated()
    {
        await using var context = Fixture.CreateContext();
        var logger = Mock.Of<ILogger<ForumThreadService>>();
        var subforumService = new Mock<ISubforumService>();
        
        await context.Database.BeginTransactionAsync();
        
        var name = "NewThreadName";
        var forumThreadId = -999;

        var threadService = new ForumThreadService(logger, context, subforumService.Object);

        var rowsUpdated = await threadService.Update(forumThreadId, name:name);
        
        Assert.Equal(0, rowsUpdated);
        
        context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task Delete_ExistingThread_ReturnTrue()
    {
        await using var context = Fixture.CreateContext();
        var logger = Mock.Of<ILogger<ForumThreadService>>();
        var subforumService = new Mock<ISubforumService>();
        
        await context.Database.BeginTransactionAsync();

        var targetSubforumId = 1;
        
        var oldThreadsList = await context.Threads
            .Where(t => t.SubforumId == targetSubforumId)
            .ToListAsync();

        var forumThreadService = new ForumThreadService(logger, context, subforumService.Object);
        
        // delete thread 1 which is part of subforum 1
        var deleted = await forumThreadService.Delete(1);
        
        Assert.True(deleted);

        var newThreadsList = await context.Threads
            .Where(t => t.SubforumId == targetSubforumId)
            .ToListAsync();
        
        Assert.Equal(oldThreadsList.Count - 1, newThreadsList.Count);
    }

    [Fact]
    public async Task Delete_NonExistingThread_ReturnFalse()
    {
        await using var context = Fixture.CreateContext();
        var logger = Mock.Of<ILogger<ForumThreadService>>();
        var subforumService = new Mock<ISubforumService>();
        
        await context.Database.BeginTransactionAsync();
        
        var forumThreadService = new ForumThreadService(logger, context, subforumService.Object);
        
        var deleted = await forumThreadService.Delete(-999);
        
        Assert.False(deleted);
    }
}