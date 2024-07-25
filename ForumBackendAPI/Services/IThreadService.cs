using ForumBackendAPI.Models;

namespace ForumBackendAPI.Services;

public interface IThreadService
{
    public IEnumerable<ForumThread> Get(int forumId);
}