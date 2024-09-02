using ForumBackendAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackendAPI.Services;

public class ThreadService(ILogger<ThreadService> logger, 
    ForumContext context,
    ISubforumService subforumService) : IThreadService
{
    public IEnumerable<ForumThread> Get(int forumId)
    {
        logger.LogInformation("Getting threads");
        return context.Threads;
    }

    public ActionResult<ForumThread> Create(string name, string? description, string author, string subforumName)
    {
        logger.LogInformation("Creating thread");
        var subforumId = subforumService.IdFromSubforumName(subforumName);
        ForumThread thread = new ()
        {
            Name = name,
            Description = description,
            Author = author,
            SubforumId = subforumId
        };
        
        context.Threads.Add(thread);
        context.SaveChanges();
        
        return thread;
    }
}