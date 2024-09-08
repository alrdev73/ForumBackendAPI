using ForumBackendAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackendAPI.Services;

public interface IThreadService
{
    Task<IEnumerable<ForumThread>> GetAll(int forumId);
    Task<ForumThread?> Create(string name, string? description, string author, string subforumName);
}