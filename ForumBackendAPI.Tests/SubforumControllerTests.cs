using ForumBackendAPI.Controllers;
using ForumBackendAPI.Models;
using ForumBackendAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ForumBackendAPITest;

public class SubforumControllerTests
{
    private const int TestCategoryId = 999;
    private readonly Mock<ISubforumService> _mockService;
    private readonly Mock<ILogger<SubforumController>> _mockLogger;

    public SubforumControllerTests()
    {
        _mockLogger = new Mock<ILogger<SubforumController>>();
        _mockService = new Mock<ISubforumService>();
    }

    private static List<Subforum> GetTestSubforums()
    {
        return
        [
            new Subforum
            {
                Name = "TestSubforum1",
                CategoryId = TestCategoryId
            },
            new Subforum
            {
                Name = "TestSubforum2",
                CategoryId = TestCategoryId
            }
        ];
    }
    
    [Fact]
    public async Task GetAll_WithSubforums_ReturnsOkWithSubforums()
    {
        _mockService.Setup(service => service.GetAll(TestCategoryId))
            .ReturnsAsync(GetTestSubforums());
        var controller = new SubforumController(_mockLogger.Object, _mockService.Object);

        var result = await controller.GetAll(TestCategoryId);

        var actionResult = Assert.IsType<OkObjectResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Subforum>>(actionResult.Value);
        Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task GetAll_WithZeroSubforums_ReturnsNoContent()
    {
        _mockService.Setup(service => service.GetAll(TestCategoryId))
            .ReturnsAsync(new List<Subforum>());
        var controller = new SubforumController(_mockLogger.Object, _mockService.Object);

        var result = await controller.GetAll(TestCategoryId);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Create_WithInvalidRequest_ReturnsBadRequest()
    {
        string testName = string.Empty;
        string description = string.Empty;
        int categoryId = 1;
        var requestBody = new SubforumController.CreateSubforumRequest(description, testName, categoryId);
        _mockService.Setup(service => service.Create(testName, description, categoryId))
            .ReturnsAsync(false);
        var controller = new SubforumController(_mockLogger.Object, _mockService.Object);

        var result = await controller.Create(requestBody);

        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreated()
    {
        string testName = "TestSubforumName";
        string description = "TestSubforumDescription";
        int categoryId = 1;
        var requestBody = new SubforumController.CreateSubforumRequest(description, testName, categoryId);
        _mockService.Setup(service => service.Create(testName, description, categoryId))
            .ReturnsAsync(true);
        var controller = new SubforumController(_mockLogger.Object, _mockService.Object);

        var result = await controller.Create(requestBody);

        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public async Task Update_WithValidRequest_ReturnsOk()
    {
        int id = 1;
        Subforum subforum = new()
        {
            SubforumId = id,
            Name = "TestName",
            Description = "TestDescription"
        };
        // pretend that some row is updated, this test is just for the status code
        _mockService.Setup(service => service.Update(id, subforum.Name, subforum.Description))
            .ReturnsAsync(1);
        var controller = new SubforumController(_mockLogger.Object, _mockService.Object);

        var result = await controller.Update(id, subforum);

        var actionResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Subforum updated successfully.", actionResult.Value);
    }

    [Fact]
    public async Task Delete_ExistingEntity_ReturnOk()
    {
        _mockService.Setup(s => s.Delete(1))
            .ReturnsAsync(true);
        var controller = new SubforumController(_mockLogger.Object, _mockService.Object);

        var result = await controller.Delete(1);

        var actionResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Subforum deleted successfully.", actionResult.Value);
    }

    [Fact]
    public async Task Delete_NonExistingCategory_ReturnBadRequest()
    {
        _mockService.Setup(s => s.Delete(1))
            .ReturnsAsync(false);
        var controller = new SubforumController(_mockLogger.Object, _mockService.Object);

        var result = await controller.Delete(1);

        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Subforum deletion failed.", actionResult.Value);
    }
    
}