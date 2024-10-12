using ForumBackendAPI.Controllers;
using ForumBackendAPI.Models;
using ForumBackendAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ForumBackendAPITest;

public class CategoryControllerTests
{
    private IEnumerable<Category> GetTestCategories()
    {
        List<Category> categories =
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
        
        return categories;
    }
    
    [Fact]
    public async Task Get_WithTwoCategories_ReturnsCategoriesWithOk()
    {
        var mockService = new Mock<ICategoryService>();
        mockService.Setup(service => service.Get())
            .ReturnsAsync(GetTestCategories());
        var controller = new CategoryController(mockService.Object);

        var result = await controller.Get();

        var actionResult = Assert.IsType<OkObjectResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Category>>(actionResult.Value);
        Assert.Equal(2, model.Count());
    }
    
    [Fact]
    public async Task Get_WithNoCategories_ReturnsNoContent()
    {
        var mockService = new Mock<ICategoryService>();
        mockService.Setup(service => service.Get())
            .ReturnsAsync(new List<Category>());
        var controller = new CategoryController(mockService.Object);

        var result = await controller.Get();

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Create_WithUnnamedCategory_ReturnsBadRequest()
    {
        var testCategory = new Category();
        
        var mockService = new Mock<ICategoryService>();
        mockService.Setup(service => service.Create(testCategory))
            .Throws<Exception>();
        var controller = new CategoryController(mockService.Object);

        var result = await controller.Create(testCategory);

        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task Create_WithCategoryNameLongerThan50Chars_ReturnsBadRequest()
    {
        var testCategory = new Category()
        {
            Name = "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO"
        };
        
        var mockService = new Mock<ICategoryService>();
        mockService.Setup(service => service.Create(testCategory))
            .Throws<Exception>();
        var controller = new CategoryController(mockService.Object);

        var result = await controller.Create(testCategory);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_WithValidCategory_ReturnsCreatedAtAction()
    {
        var testCategory = new Category()
        {
            Name = "TestCreatedCategory"
        };

        var mockService = new Mock<ICategoryService>();
        mockService.Setup(service => service.Create(testCategory))
            .ReturnsAsync(testCategory);
        var controller = new CategoryController(mockService.Object);

        var result = await controller.Create(testCategory);

        Assert.IsType<CreatedAtActionResult>(result);
    }
}