using ForumBackendAPI.Controllers;
using ForumBackendAPI.Models;
using ForumBackendAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ForumBackendAPITest;

public class ForumThreadControllerTests
{
    private readonly Mock<IForumThreadService> _mockService;
    private readonly Mock<ILogger<ForumThreadController>> _mockLogger;

    public ForumThreadControllerTests()
    {
        _mockService = new Mock<IForumThreadService>();
        _mockLogger = new Mock<ILogger<ForumThreadController>>();
    }
    
    private static List<ForumThread> GetTestThreads()
    {
        return 
        [
            new ForumThread
            {
                SubforumId = 1,
                Name = "Thread1",
                Author = "Author1"
            },
            new ForumThread
            {
                SubforumId = 1,
                Name = "Thread2",
                Author = "Author2"
            }
        ];
    }

    [Fact]
    public async Task GetAll_WithExistingThreads_ReturnsOkWithThreads()
    {
        _mockService.Setup(service => service.GetAll(1))
            .ReturnsAsync(GetTestThreads());
        var controller = new ForumThreadController(_mockLogger.Object, _mockService.Object);

        var result = await controller.GetAll(1);

        var actionResult = Assert.IsType<OkObjectResult>(result);
        var model = Assert.IsAssignableFrom<IList<ForumThread>>(actionResult.Value);
        Assert.Equal(2, model.Count);
    }
    
    [Fact]
    public async Task GetAll_WithNoThreads_ReturnsNoContent()
    {
        _mockService.Setup(service => service.GetAll(1))
            .ReturnsAsync(new List<ForumThread>());
        var controller = new ForumThreadController(_mockLogger.Object, _mockService.Object);

        var result = await controller.GetAll(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Create_WithInvalidRequest_ReturnsBadRequest()
    {
        const string name = "TestName";
        const string description = "TestDescription";
        const string author = "TestAuthor";
        const int subforumId = 1;
        
        _mockService.Setup(service => service.Create(name, description, author, subforumId))
            .ReturnsAsync(false);
        var requestBody = new ForumThreadController.ThreadCreateRequest(name, description, author, subforumId);
        var controller = new ForumThreadController(_mockLogger.Object, _mockService.Object);

        var result = await controller.CreateThread(requestBody);

        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreated()
    {
        const string name = "TestName";
        const string description = "TestDescription";
        const string author = "TestAuthor";
        const int subforumId = 1;
        
        _mockService.Setup(service => service.Create(name, description, author, subforumId))
            .ReturnsAsync(true);
        var requestBody = new ForumThreadController.ThreadCreateRequest(name, description, author, subforumId);
        var controller = new ForumThreadController(_mockLogger.Object, _mockService.Object);

        var result = await controller.CreateThread(requestBody);

        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public async Task Update_WithValidRequest_ReturnsOk()
    {
        int id = 1;
        var forumThread = new ForumThread
        {
            ForumThreadId = id,
            Name = "UpdatedName",
            Description = "UpdatedDescription"
        };

        // pretend that the service updates the thread, returning the 2 rows updated.
        _mockService.Setup(s => s.Update(forumThread.ForumThreadId, forumThread.Name, forumThread.Description))
            .ReturnsAsync(2);
        
        var controller = new ForumThreadController(_mockLogger.Object, _mockService.Object);

        var result = await controller.Update(id, forumThread);

        var actionResult = Assert.IsType<OkObjectResult>(result);
        
        Assert.Equal("Forum thread updated successfully.", actionResult.Value);
    }

    [Fact]
    public async Task Delete_ExistingForumThread_ReturnsOk()
    {
        int id = 1;
        
        // pretend that the thread exists
        _mockService.Setup(s => s.Delete(id))
            .ReturnsAsync(true);
        
        var controller = new ForumThreadController(_mockLogger.Object, _mockService.Object);

        var result = await controller.Delete(id);

        var actionResult = Assert.IsType<OkObjectResult>(result);
        
        Assert.Equal("Forum thread deleted successfully.", actionResult.Value);
    }
    
    [Fact]
    public async Task Delete_NonExistingForumThread_ReturnsBadRequest()
    {
        int id = 1;
        
        // pretend that the thread does not exist
        _mockService.Setup(s => s.Delete(id))
            .ReturnsAsync(false);
        
        var controller = new ForumThreadController(_mockLogger.Object, _mockService.Object);

        var result = await controller.Delete(id);

        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        
        Assert.Equal("Forum thread does not exist.", actionResult.Value);
    }
}