﻿using ForumBackendAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForumBackendAPI.Services;

public class SubforumService(ILogger<SubforumService> logger, ForumContext context, ICategoryService categoryService) : ISubforumService
{
    public async Task<IEnumerable<Subforum>> GetAll([FromRoute] int categoryId)
    {
        logger.LogInformation("Getting subforums");
        var subforums = await context.Subforums
            .Where(c => c.CategoryId == categoryId)
            .AsNoTracking()
            .ToListAsync();

        return subforums;
    }
    
    public async Task<string> NameFromSubforumId(int subforumId)
    {
        var subforum = await context.Subforums
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SubforumId == subforumId);
        
        return subforum == null ? string.Empty : subforum.Name;
    }

    public async Task<bool> Exists(int subforumId)
    {
        var subforum = await context.Subforums
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SubforumId == subforumId);

        return subforum == null;
    }

    public async Task<Subforum?> Create(string name, string description, int categoryId)
    {
        logger.LogInformation("Creating subforum");
        var categoryExists = await categoryService.Exists(categoryId);
        
        if (!categoryExists)
        {
            // category doesn't exist, abort subforum creation
            return null;
        }
        
        var subforum = new Subforum
        {
            Name = name,
            Description = description,
            CategoryId = categoryId
        };
        
        context.Subforums.Add(subforum);
        await context.SaveChangesAsync();
        
        return subforum;
    }
    
  
    public async Task<Subforum> Update(int subforumId, Subforum subforum)
    {
        logger.LogInformation("Updating subforum");

        await context.Subforums
            .Where(c => c.SubforumId == subforumId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.Name, subforum.Name)
                .SetProperty(d => d.Description, subforum.Description));
        
        return subforum;
    }
    
}