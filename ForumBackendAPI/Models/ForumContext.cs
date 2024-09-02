using Microsoft.EntityFrameworkCore;

namespace ForumBackendAPI.Models;

public class ForumContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Subforum> Subforums { get; set; }
    public DbSet<ForumThread> Threads { get; set; }

    private readonly IConfiguration _configuration;
    
    public ForumContext(DbContextOptions<ForumContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Default"));
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().ToTable("Categories");
        modelBuilder.Entity<Subforum>().ToTable("Subforums");
        modelBuilder.Entity<ForumThread>().ToTable("Threads");
    }
}