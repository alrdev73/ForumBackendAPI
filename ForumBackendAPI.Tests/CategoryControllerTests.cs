using ForumBackendAPI.Controllers;
using ForumBackendAPI.Models;
using ForumBackendAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ForumBackendAPITest;

public class CategoryControllerTests
{
    private readonly Mock<ICategoryService> _mockService;
    private readonly Mock<ILogger<CategoryController>> _mockLogger;

    public CategoryControllerTests()
    {
        _mockService = new Mock<ICategoryService>();
        _mockLogger = new Mock<ILogger<CategoryController>>();
    }
    
    private static List<Category> GetTestCategories()
    {
        return
        [
            new Category
            {
                Name = "Category1"
            },

            new Category
            {
                Name = "Category2"
            }
        ];
    }
    
    [Fact]
    public async Task Get_WithTwoCategories_ReturnsCategoriesWithOk()
    {
        _mockService.Setup(service => service.GetAll())
            .ReturnsAsync(GetTestCategories());
        var controller = new CategoryController(_mockLogger.Object, _mockService.Object);

        var result = await controller.Get();

        var actionResult = Assert.IsType<OkObjectResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Category>>(actionResult.Value);
        Assert.Equal(2, model.Count());
    }
    
    [Fact]
    public async Task Get_WithNoCategories_ReturnsNoContent()
    {
        _mockService.Setup(service => service.GetAll())
            .ReturnsAsync(new List<Category>());
        var controller = new CategoryController(_mockLogger.Object, _mockService.Object);

        var result = await controller.Get();

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Create_WithValidCategory_ReturnsCreatedAtAction()
    {
        var testCategory = new Category()
        {
            Name = "TestCreatedCategory"
        };
        _mockService.Setup(service => service.Create(testCategory))
            .ReturnsAsync(true);
        var controller = new CategoryController(_mockLogger.Object, _mockService.Object);

        var result = await controller.Create(testCategory);

        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public async Task Rename_ExistingCategoryValidName_ReturnsOk()
    {
        var categoryId = 1;
        var name = "TestRenameCategory";
        // pretend that it updates the name and returns 1 row updated
        _mockService.Setup(service => service.Rename(categoryId, name))
            .ReturnsAsync(1);
        var controller = new CategoryController(_mockLogger.Object, _mockService.Object);

        var result = await controller.Rename(name, categoryId);

        var response = Assert.IsType<OkObjectResult>(result);
        
        Assert.Equal("Category renamed successfully.", response.Value);
    }

    [Fact]
    public async Task Rename_ServiceThrowsException_ServerErr()
    {
        var categoryId = 1;
        var name = "InvalidName0000000000000000000000000000000000000000000000000000000000000";
        _mockService.Setup(service => service.Rename(categoryId, name))
            .Throws<Exception>();
        var controller = new CategoryController(_mockLogger.Object, _mockService.Object);

        var result = await controller.Rename(name, categoryId);

        var response = Assert.IsType<StatusCodeResult>(result);
        
        Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ServiceDeletesSuccessfully_Ok()
    {
        var categoryId = 1;
        _mockService.Setup(service => service.Delete(categoryId))
            .ReturnsAsync(true);
        var controller = new CategoryController(_mockLogger.Object, _mockService.Object);

        var result = await controller.Delete(categoryId);

        var response = Assert.IsType<OkObjectResult>(result);

        var model = Assert.IsAssignableFrom<string>(response.Value);
        
        Assert.Equal("Category deleted successfully.", model);
    }

    [Fact]
    public async Task Delete_CategoryDidNotExist_BadRequest()
    {
        var categoryId = -999;
        _mockService.Setup(service => service.Delete(categoryId))
            .ReturnsAsync(false);
        var controller = new CategoryController(_mockLogger.Object, _mockService.Object);

        var result = await controller.Delete(categoryId);

        var response = Assert.IsType<BadRequestObjectResult>(result);

        var model = Assert.IsAssignableFrom<string>(response.Value);
        
        Assert.Equal("Category could not be removed as it does not exist.", model);
    }

    [Fact]
    public async Task Delete_ServiceThrowsException_ServerErr()
    {
        var categoryId = 1;
        _mockService.Setup(service => service.Delete(categoryId))
            .Throws<Exception>();
        var controller = new CategoryController(_mockLogger.Object, _mockService.Object);

        var result = await controller.Delete(categoryId);

        var response = Assert.IsType<StatusCodeResult>(result);
        
        Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
    }
}