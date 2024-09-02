using ForumBackendAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackendAPI.Services;

public interface IThreadService
{
    public IEnumerable<ForumThread> Get(int forumId);

    public ActionResult<ForumThread> Create(string name, string? description, string author, string subforumName);
}