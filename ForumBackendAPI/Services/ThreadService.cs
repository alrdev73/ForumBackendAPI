using ForumBackendAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForumBackendAPI.Services;

public class ThreadService(ILogger<ThreadService> logger, 
    ForumContext context,
    ISubforumService subforumService) : IThreadService
{
    public async Task<IEnumerable<ForumThread>> GetAll(int forumId)
    {
        logger.LogInformation("Getting threads");

        var threads = await context.Threads
            .Where(t => t.SubforumId == forumId)
            .AsNoTracking()
            .ToListAsync();
        
        return threads;
    }

    public async Task<ForumThread?> Create(string name, string? description, string author, string subforumName)
    {
        logger.LogInformation("Creating thread");
        
        var subforumId = await subforumService.IdFromSubforumName(subforumName);

        if (subforumId == int.MinValue)
        {
            // the subforum doesn't exist, abort thread creation.
            return null;
        }
        
        ForumThread thread = new ()
        {
            Name = name,
            Description = description,
            Author = author,
            SubforumId = subforumId
        };
        
        context.Threads.Add(thread);
        await context.SaveChangesAsync();
        
        return thread;
    }
}