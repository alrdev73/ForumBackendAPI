using ForumBackendAPI.Models;

namespace ForumBackendAPI.Services;

public interface ICategoryService
{
    Task<bool> Create(Category category);
    Task<IList<Category>> GetAll();
    Task<int> Rename(int categoryId, string name);
    Task<bool> Delete(int categoryId);
    Task<Category?> GetCategoryForId(int categoryId);
}