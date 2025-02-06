using ForumBackendAPI.Models;
using ForumBackendAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackendAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoryController(ILogger<CategoryController> logger, ICategoryService categoryService) : ControllerBase
{
    [HttpGet(Name = "GetCategories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Get()
    {
        var categories = await categoryService.GetAll();

        if (categories.Count == 0)
        {
            return NoContent();
        }

        return Ok(categories);
    }

    [HttpPost(Name = "CreateCategory")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] Category category)
    {
        try
        {
            var success = await categoryService.Create(category);
            if (!success)
            {
                // something went wrong when the SQL was executed
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Created();
        }
        catch (Exception ex)
        {
            logger.LogError("Exception occurred on category Create: {ex}", ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPatch(Name = "RenameCategory")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Rename(string name, int categoryId)
    {
        try
        {
            await categoryService.Rename(categoryId, name);
            return Ok("Category renamed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError("Exception occurred on category Rename: {ex}", ex);
            return StatusCode(500);
        }
    }

    [HttpDelete(Name = "DeleteCategory")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromBody] int categoryId)
    {
        try
        {
            var deleted = await categoryService.Delete(categoryId);
            if (deleted)
            {
                return Ok("Category deleted successfully.");
            }

            return BadRequest("Category could not be removed as it does not exist.");
        }
        catch (Exception ex)
        {
            logger.LogError("Exception occurred on Category delete: {ex}", ex);
            return StatusCode(500);
        }
    }
}