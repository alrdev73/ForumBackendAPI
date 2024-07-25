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
    public IEnumerable<Category> Get() => categoryService.Get();

    [HttpPost(Name = "CreateCategory")]
    public ActionResult<Category> Create([FromBody] Category category) => categoryService.Create(category);
}