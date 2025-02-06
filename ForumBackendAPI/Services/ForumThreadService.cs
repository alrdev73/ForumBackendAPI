using ForumBackendAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForumBackendAPI.Services;

public class ForumThreadService(ILogger<ForumThreadService> logger, ForumContext context, ISubforumService subforumService) : IForumThreadService
{
    public async Task<IList<ForumThread>> GetAll(int subforumId)
    {
        logger.LogInformation("Getting threads");

        var threads = await context.Threads
            .Where(t => t.SubforumId == subforumId)
            .AsNoTracking()
            .ToListAsync();
        
        return threads;
    }

    public Task<ForumThread> GetThreadForId(int forumThreadId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> Create(string name, string description, string author, int subforumId)
    {
        logger.LogInformation("Creating thread");
        
        var subforum = await subforumService.GetSubforumForId(subforumId);

        if (subforum == null)
        {
            // the subforum doesn't exist, abort thread creation.
            return false;
        }

        try
        {
            ForumThread thread = new ()
            {
                Name = name,
                Description = description,
                Author = author,
                SubforumId = subforumId,
                Subforum = subforum
            };
            
            context.Threads.Add(thread);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("Exception when creating thread: {e}", e);
            return false;
        }
    }

    public async Task<int> Update(int forumThreadId, string name = "", string description = "")
    {
        logger.LogInformation("Updating thread.");

        var rowsUpdated = await context.Threads
            .Where(t => t.ForumThreadId == forumThreadId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.Name, n => name != "" && !n.Name.Equals(name) ? name : n.Name)
                .SetProperty(d => d.Description,
                    d => description != "" && !d.Description.Equals(description) ? description : d.Description));
        
        logger.LogDebug("Renamed thread with id {forumThreadId}", forumThreadId);
        return rowsUpdated;
    }

    public async Task<bool> Delete(int forumThreadId)
    {
        logger.LogInformation("Deleting a thread.");

        var forumThread = await context.Threads.FindAsync(forumThreadId);

        if (forumThread == null)
        {
            // doesn't exist
            return false;
        }

        try
        {
            context.Remove(forumThread);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("Exception when deleting thread: {e}", e);
            return false;
        }
    }
}