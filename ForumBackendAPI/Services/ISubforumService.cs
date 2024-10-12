using ForumBackendAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackendAPI.Services;

public interface ISubforumService
{
    Task<IEnumerable<Subforum>> GetAll(int categoryId);
    Task<Subforum?> Create(string name, string description, int categoryId);
    Task<Subforum> Update(int subforumId, Subforum subforum);
    Task<string> NameFromSubforumId(int subforumId);

    Task<bool> Exists(int subforumId);
}