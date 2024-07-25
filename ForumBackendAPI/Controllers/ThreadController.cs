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
}