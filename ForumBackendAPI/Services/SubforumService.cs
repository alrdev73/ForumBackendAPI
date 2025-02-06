using ForumBackendAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForumBackendAPI.Services;

public class SubforumService(ILogger<SubforumService> logger, ForumContext context, ICategoryService categoryService) : ISubforumService
{
    
    public async Task<bool> Create(string name, string description, int categoryId)
    {
        logger.LogInformation("Creating subforum");
        var category = await categoryService.GetCategoryForId(categoryId);
        
        if (category == null)
        {
            // category doesn't exist, abort subforum creation.
            return false;
        }

        try
        {
            var subforum = new Subforum
            {
                Name = name,
                Description = description,
                CategoryId = categoryId,
                Category = category
            };
        
            context.Subforums.Add(subforum);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        { 
            logger.LogError("Exception thrown when adding subforum: {e}", e);
            return false;
        }
    }
    
    public async Task<IList<Subforum>> GetAll([FromRoute] int categoryId)
    {
        logger.LogInformation("Getting subforums");
        return await context.Subforums
            .Where(c => c.CategoryId == categoryId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Subforum?> GetSubforumForId(int subforumId)
    {
        return await context.Subforums.FindAsync(subforumId);
    }

    public async Task<int> Update(int subforumId, string name = "", string description = "")
    {
        logger.LogInformation("Updating subforum with id {subforumId}", subforumId);
        
        var rowsUpdated = await context.Subforums
            .Where(c => c.SubforumId == subforumId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.Name, n => name != "" && !name.Equals(n.Name) ? name : n.Name)
                .SetProperty(d => d.Description, d => description != "" && !description.Equals(d.Description) ? description : d.Description));
        
        logger.LogDebug("Renamed subforum with id {subforumId}. Rows updated: {rowsUpdated}", subforumId, rowsUpdated);
        return rowsUpdated;
    }

    public async Task<bool> Delete(int subforumId)
    {
        logger.LogInformation("Deleting subforum");
        
        var subforum = await context.Subforums.FindAsync(subforumId);

        if (subforum == null)
        {
            // subforum doesn't exist, so it cannot be deleted
            return false;
        }
        
        try
        {
            context.Remove(subforum);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("Exception when deleting category: {e}", e);
            return false;
        }    
    }
}