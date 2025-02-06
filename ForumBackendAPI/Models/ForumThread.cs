using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ForumBackendAPI.Models;

public class ForumThread
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ForumThreadId { get; set; }

    [MaxLength(50)] [Required] public string Name { get; set; } = "noname";

    [MaxLength(100)] public string Description { get; set; } = "";

    [MaxLength(50)] [Required] public string Author { get; set; } = "noname";
 
    public DateTime Date { get; set; } = DateTime.UtcNow;
    
    public int Replies { get; set; } 
    
    public int Views { get; set; } 
    
    public int SubforumId { get; set; }
    
    [JsonIgnore]
    public virtual Subforum Subforum { get; set; }

    // public IList<ForumPost> posts = new List<ForumPost>();
}