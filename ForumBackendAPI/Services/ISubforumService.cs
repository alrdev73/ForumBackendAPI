using ForumBackendAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackendAPI.Services;

public interface ISubforumService
{
    public IEnumerable<Subforum> Get(int categoryId);
    public Subforum Create(string name, string description, string categoryName);
    
    public string NameFromSubforumId(int subforumId);
    ActionResult<Subforum> Update(int subforumId, Subforum subforum);
}