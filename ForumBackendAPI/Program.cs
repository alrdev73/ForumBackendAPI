using ForumBackendAPI.Models;
using ForumBackendAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var specificOrigin = "AllowVite";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: specificOrigin,
        policy =>
        {
            // vite
            policy.WithOrigins("https://localhost:5173");
        });
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ForumContext>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IThreadService, ThreadService>();
builder.Services.AddScoped<ISubforumService, SubforumService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     var context = services.GetRequiredService<ForumContext>();
//     if (context.Database.GetPendingMigrations().Any())
//     {
//         context.Database.Migrate();
//     }
// }

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(specificOrigin);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();