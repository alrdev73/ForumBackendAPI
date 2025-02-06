using ForumBackendAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackendAPI.Services;

public interface IForumThreadService
{
    Task<IList<ForumThread>> GetAll(int subforumId);
    Task<ForumThread> GetThreadForId(int forumThreadId);
    Task<bool> Create(string name, string description, string author, int subforumId);
    Task<int> Update(int forumThreadId, string name, string description);
    Task<bool> Delete(int forumThreadId);
}