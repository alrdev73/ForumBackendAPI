using ForumBackendAPI.Models;

namespace ForumBackendAPI.Services;

public interface ICategoryService
{
    Task<IEnumerable<Category>> Get();
    Task<Category> Create(Category category);
    Task<bool> Exists(int subforumId);
}