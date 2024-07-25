using ForumBackendAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackendAPI.Services;

public class SubforumService(ILogger<SubforumService> logger, ForumContext context, ICategoryService categoryService) : ISubforumService
{
    public IEnumerable<Subforum> Get([FromRoute] int categoryId)
    {
        logger.LogInformation("Getting subforums");
        return context.Subforums.Where(c => c.CategoryId == categoryId).ToList();
    }
    
    public string NameFromSubforumId(int subforumId)
    {
        return context.Subforums.First(c => c.SubforumId == subforumId).Name;
    }
    
    public Subforum Create(string name, string description, string categoryName)
    {
        logger.LogInformation("Creating subforum");
        var categoryId = categoryService.CategoryIdFromName(categoryName);
        var subforum = new Subforum
        {
            Name = name,
            Description = description,
            CategoryId = categoryId
        };
        
        context.Subforums.Add(subforum);
        context.SaveChanges();
        return subforum;        
    }
    
    public ActionResult<Subforum> Update(int subforumId, Subforum subforum)
    {
        logger.LogInformation("Updating subforum");
        var existingSubforum = context.Subforums.First(c => c.SubforumId == subforumId);
        existingSubforum.Name = subforum.Name;
        existingSubforum.Description = subforum.Description;
        context.SaveChanges();
        return existingSubforum;
    }
}