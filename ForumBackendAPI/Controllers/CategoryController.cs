using ForumBackendAPI.Models;
using ForumBackendAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackendAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet(Name = "GetCategories")]
    public async Task<IActionResult> Get()
    {
        var categories = await categoryService.Get();

        return Ok(categories);
    }

    [HttpPost(Name = "CreateCategory")]
    public IActionResult Create([FromBody] Category category)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Category name is too long (maximum 50 characters).");
        }

        var cat = categoryService.Create(category);
        
        return Ok(cat);
    }
}