using ForumBackendAPI.Models;
using ForumBackendAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackendAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SubforumController(ISubforumService subforumService) : ControllerBase
{
    [HttpGet("{categoryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAll(int categoryId)
    {
        var subforums = await subforumService.GetAll(categoryId);

        if (!subforums.Any())
        {
            return NoContent();
        }
        
        return Ok(subforums);
    }
    
    [HttpGet("Name/{subforumId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Name(int subforumId)
    {
        var name = await subforumService.NameFromSubforumId(subforumId);

        if (name == string.Empty)
        {
            return NotFound();
        }
        
        return Ok(name);
    }

    public readonly struct CreateSubforumRequest(string description, string name, int categoryId)
    {
        public string Name { get; } = name;
        public string Description { get; } = description;
        public int CategoryId { get; } = categoryId;
    }
    
    [HttpPost(Name = "CreateSubforum")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSubforumRequest request)
    {
        try
        {
            var created = await subforumService.Create(request.Name, request.Description, request.CategoryId);

            if (created == null)
            {
                return BadRequest("Subforum creation failed.");
            }
            
            return CreatedAtAction(nameof(Name), new { id = created.SubforumId}, created);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    
    [HttpPut("{subforumId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int subforumId, [FromBody] Subforum subforum)
    {
        try
        {
            var updated = await subforumService.Update(subforumId, subforum);
            
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}