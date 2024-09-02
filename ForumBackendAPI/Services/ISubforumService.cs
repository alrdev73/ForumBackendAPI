using ForumBackendAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackendAPI.Services;

public interface ISubforumService
{
    public IEnumerable<Subforum> Get(int categoryId);
    public Subforum Create(string name, string description, string categoryName);
    
    public int IdFromSubforumName(string subforumName);
    ActionResult<Subforum> Update(int subforumId, Subforum subforum);
    string NameFromSubforumId(int subforumId);
}