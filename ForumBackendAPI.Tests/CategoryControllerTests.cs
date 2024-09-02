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
    public async Task Get_ReturnsCategoriesWithOk_TwoCategories()
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
    public async Task Get_ReturnsCategoriesWithOk_ZeroCategories()
    {
        var mockService = new Mock<ICategoryService>();
        mockService.Setup(service => service.Get())
            .ReturnsAsync(new List<Category>());
        var controller = new CategoryController(mockService.Object);

        var result = await controller.Get();

        var actionResult = Assert.IsType<OkObjectResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Category>>(actionResult.Value);
        Assert.Empty(model);
    }
}