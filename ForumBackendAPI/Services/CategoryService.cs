using ForumBackendAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ForumBackendAPI.Services;

public class CategoryService(ILogger<CategoryService> logger, ForumContext context) : ICategoryService
{
    public async Task<string> NameFromCategoryId(int categoryId)
    {
        var category = await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        
        return category == null ? string.Empty : category.Name;
    }
    
    public async Task<bool> Exists(int categoryId)
    {
        var category = await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

        return category == null;
    }

    public async Task<IEnumerable<Category>> Get()
    {
        logger.LogInformation("Getting categories");
        var categories = await context.Categories
            .AsNoTracking()
            .ToListAsync();
        
        return categories;
    }
    
    public async Task<Category> Create(Category category)
    {
        logger.LogInformation("Creating category");
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }
}