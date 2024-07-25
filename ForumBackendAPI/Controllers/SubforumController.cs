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
    public IEnumerable<Subforum> Get(int categoryId)
    {
        return subforumService.Get(categoryId);
    }
    
    [HttpGet("Name/{subforumId}")]
    public string Name(int subforumId)
    {
        return subforumService.NameFromSubforumId(subforumId);
    }

    public struct CreateSubforumRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
    }
    
    [HttpPost(Name = "CreateSubforum")]
    public ActionResult<Subforum> Create([FromBody] CreateSubforumRequest request)
    {
        return subforumService.Create(request.Name, request.Description, request.CategoryName);
    }
    
    [HttpPut("{subforumId}")]
    public ActionResult<Subforum> Update(int subforumId, [FromBody] Subforum subforum)
    {
        return subforumService.Update(subforumId, subforum);
    }
}