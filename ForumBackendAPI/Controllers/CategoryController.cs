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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Get()
    {
        var categories = await categoryService.Get();

        if (!categories.Any())
        {
            return NoContent();
        }

        return Ok(categories);
    }

    [HttpPost(Name = "CreateCategory")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] Category category)
    {
        try
        {
            var created = await categoryService.Create(category);
            return CreatedAtAction(nameof(Get), new { id = created.CategoryId }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}