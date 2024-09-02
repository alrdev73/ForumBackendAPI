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
    public IEnumerable<ForumThread> Get(int forumId)
    {
        return threadService.Get(forumId);
    }

    public struct ThreadCreateRequest
    {
        public string Name { get; }
        
        public string? Description { get; }
        
        public string Author { get; }
        
        public string SubforumName { get; }
    }
    

    [HttpPost(Name = "CreateThread")]
    public ActionResult<ForumThread> CreateThread([FromBody] ThreadCreateRequest thread)
    {
        return threadService.Create(thread.Name, thread.Description, thread.Author, thread.SubforumName);
    }
}