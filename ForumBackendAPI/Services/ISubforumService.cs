using ForumBackendAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackendAPI.Services;

public interface ISubforumService
{
    Task<bool> Create(string name, string description, int categoryId);
    Task<IList<Subforum>> GetAll(int categoryId);
    Task<Subforum?> GetSubforumForId(int subforumId);
    Task<int> Update(int subforumId, string name, string description);
    Task<bool> Delete(int subforumId);
}