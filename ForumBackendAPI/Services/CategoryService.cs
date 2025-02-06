using ForumBackendAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ForumBackendAPI.Services;

public class CategoryService(ILogger<CategoryService> logger, ForumContext context) : ICategoryService
{
    
    public async Task<bool> Create(Category category)
    {
        logger.LogInformation("Creating category");
        
        try
        {
            context.Categories.Add(category);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("Error when adding category to the database: {e}", e);
            return false;
        }
    }
    
    public async Task<IList<Category>> GetAll()
    {
        logger.LogInformation("Getting categories");

        var categories = await context.Categories
            .Include(c => c.Subforums)
            .AsNoTracking()
            .ToListAsync();
        
        return categories;
    }

    public async Task<int> Rename(int categoryId, string name)
    {
        logger.LogInformation("Updating category with id {categoryId}", categoryId);

        if (string.IsNullOrEmpty(name)) return 0;
        
        var rowsUpdated = await context.Categories
            .Where(c => c.CategoryId == categoryId)
            .ExecuteUpdateAsync(c => c
                .SetProperty(n => n.Name, name));
        
        if(rowsUpdated > 0) logger.LogDebug("Updated category with id {categoryId}. Rows updated: {rowsUpdated}", categoryId, rowsUpdated);
        
        return rowsUpdated;
    }

    public async Task<bool> Delete(int categoryId)
    {
        logger.LogInformation("Deleting a category");
        var category = await context.Categories.FindAsync(categoryId);

        if (category == null)
        {
            // category doesn't exist
            return false;
        }
        
        try
        {
            context.Remove(category);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("Error when deleting category: {e}", e);
            return false;
        }
    }

    public async Task<Category?> GetCategoryForId(int categoryId)
    {
        return await context.Categories.FindAsync(categoryId);
    }
}