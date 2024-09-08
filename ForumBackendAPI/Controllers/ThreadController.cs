using ForumBackendAPI.Models;
using ForumBackendAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackendAPI.Controllers;

[ApiController]
[Route("api/[controller]/{forumId}")]
[Produces("application/json")]
public class ThreadController(IThreadService threadService) : ControllerBase
{
    [HttpGet(Name = "GetThreads")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAll(int forumId)
    {
        var threads = await threadService.GetAll(forumId);

        if (!threads.Any())
        {
            return NoContent();
        }
        
        return Ok(threads);
    }

    public readonly struct ThreadCreateRequest(string name, string description, string author, string subforumName)
    {
        public string Name { get; } = name;
        public string? Description { get; } = description;
        public string Author { get; } = author;
        public string SubforumName { get; } = subforumName;
    }
    

    [HttpPost(Name = "CreateThread")]
    public async Task<IActionResult> CreateThread([FromBody] ThreadCreateRequest thread)
    {
        try
        {
            var created =
                await threadService.Create(thread.Name, thread.Description, thread.Author, thread.SubforumName);

            if (created == null)
            {
                return BadRequest("Thread creation failed.");
            }
            
            return CreatedAtAction(nameof(GetAll), new { id = created.ForumThreadId}, created);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}