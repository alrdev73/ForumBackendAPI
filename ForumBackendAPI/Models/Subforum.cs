using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ForumBackendAPI.Models;

public class Subforum
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SubforumId { get; set; }
    
    [MaxLength (50)]
    [Required]
    public string Name { get; set; }
    
    [MaxLength (100)]
    public string? Description { get; set; }

    public int Replies { get; set; }
    
    public int Threads { get; set; }
    
    public int CategoryId { get; set; }
    
    [JsonIgnore]
    public virtual Category Category { get; set; }
}