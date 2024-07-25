using ForumBackendAPI.Models;

namespace ForumBackendAPI.Services;

public class ThreadService(ILogger<ThreadService> logger) : IThreadService
{
    private readonly List<ForumThread> _threads = new()
    {
        new ForumThread
        {
            Name = "Thread 1",
            Author = "Author 1",
            Date = DateTime.Now,
        },
        new ForumThread
        {
            Name = "Thread 2",
            Author = "Author 2",
            Date = DateTime.Now,
        }
    };
    
    public IEnumerable<ForumThread> Get(int forumId)
    {
        // TODO search for threads in forumId, must setup PostgreSQL first
        logger.LogInformation("Getting threads");
        // return new List<ForumThread>().ToArray();
        return _threads;
    }
}