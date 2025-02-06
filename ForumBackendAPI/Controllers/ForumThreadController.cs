using ForumBackendAPI.Models;
using ForumBackendAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackendAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ForumThreadController(ILogger<ForumThreadController> logger, IForumThreadService forumThreadService) : ControllerBase
{
    [HttpGet(Name = "GetThreads")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAll(int forumId)
    {
        var threads = await forumThreadService.GetAll(forumId);

        if (threads.Count == 0)
        {
            return NoContent();
        }
        
        return Ok(threads);
    }

    public readonly struct ThreadCreateRequest(string name, string description, string author, int subforumId)
    {
        public string Name { get; } = name;
        public string Description { get; } = description;
        public string Author { get; } = author;
        
        public int SubforumId { get; } = subforumId;
    }
    

    [HttpPost(Name = "CreateThread")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateThread([FromBody] ThreadCreateRequest thread)
    {
        try
        {
            var created = await forumThreadService.Create(thread.Name, thread.Description, thread.Author, thread.SubforumId);

            if (created == false)
            {
                return BadRequest("Thread creation failed.");
            }
            
            return Created();
        }
        catch (Exception ex)
        {
            logger.LogError("Exception when creating thread: {ex}", ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPatch("{forumThreadId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int forumThreadId, [FromBody] ForumThread forumThread)
    {
        logger.LogInformation("Updating thread.");
        try
        {
            await forumThreadService.Update(forumThreadId, forumThread.Name, forumThread.Description);
            return Ok("Forum thread updated successfully.");
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("{forumThreadId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int forumThreadId)
    {
        logger.LogInformation("Deleting thread.");

        try
        {
            var deleted = await forumThreadService.Delete(forumThreadId);

            if (!deleted)
            {
                return BadRequest("Forum thread does not exist.");
            }

            return Ok("Forum thread deleted successfully.");
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}