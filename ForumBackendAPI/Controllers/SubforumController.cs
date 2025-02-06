using ForumBackendAPI.Models;
using ForumBackendAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackendAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SubforumController(ILogger<SubforumController> logger, ISubforumService subforumService) : ControllerBase
{
    [HttpGet("{categoryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAll(int categoryId)
    {
        var subforums = await subforumService.GetAll(categoryId);

        if (subforums.Count == 0)
        {
            return NoContent();
        }
        
        return Ok(subforums);
    }

    public readonly struct CreateSubforumRequest(string description, string name, int categoryId)
    {
        public string Name { get; } = name;
        public string Description { get; } = description;
        public int CategoryId { get; } = categoryId;
    }
    
    [HttpPost(Name = "CreateSubforum")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateSubforumRequest request)
    {
        try
        {
            var created = await subforumService.Create(request.Name, request.Description, request.CategoryId);

            if (!created)
            {
                return BadRequest("Subforum creation failed.");
            }

            return Created();
        }
        catch (Exception ex)
        {
            logger.LogError("Exception occurred on Subforum create: {ex}", ex);
            return StatusCode(500);
        }
    }
    
    [HttpPatch("{subforumId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int subforumId, [FromBody] Subforum subforum)
    {
        try
        {
            await subforumService.Update(subforumId, name: subforum.Name, description: subforum.Description);
            
            return Ok("Subforum updated successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError("Exception occurred on Subforum update: {ex}", ex);
            return StatusCode(500);
        }
    }

    [HttpDelete("{subforumId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int subforumId)
    {
        logger.LogInformation("Deleting subforum with id {subforumId}", subforumId);

        try
        {
            var success = await subforumService.Delete(subforumId);

            if (!success)
            {
                return BadRequest("Subforum deletion failed.");
            }

            return Ok("Subforum deleted successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError("Exception when deleting subforum: {ex}", ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

}