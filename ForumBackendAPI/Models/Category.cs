using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ForumBackendAPI.Models;

public class Category
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CategoryId { get; init; }

    [MinLength(1)] [MaxLength(50)] public string Name { get; init; } = "noname";

    [JsonIgnore] public IList<Subforum> Subforums { get; } = new List<Subforum>();
}