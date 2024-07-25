using ForumBackendAPI.Models;

namespace ForumBackendAPI.Services;

public class CategoryService(ILogger<CategoryService> logger, ForumContext context) : ICategoryService
{
    
    public string NameFromCategoryId(int categoryId)
    {
        return context.Categories.First(c => c.CategoryId == categoryId).Name;
    }
    
    public int CategoryIdFromName(string categoryName)
    {
        return context.Categories.First(c => c.Name.ToLower() == categoryName.ToLower()).CategoryId;
    }

    public IEnumerable<Category> Get()
    {
        logger.LogInformation("Getting categories");
        var categories = context.Categories.ToList();
        return categories;
    }
    
    public Category Create(Category category)
    {
        logger.LogInformation("Creating category");
        context.Categories.Add(category);
        context.SaveChanges();
        return category;
    }
}