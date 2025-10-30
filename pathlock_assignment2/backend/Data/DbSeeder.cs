using Microsoft.EntityFrameworkCore;
using ProjectManager.Api.Models;
using System;
using System.Threading.Tasks;

namespace ProjectManager.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.EnsureCreatedAsync();

        if (!await db.Users.AnyAsync())
        {
            var demoUser = new User
            {
                Username = "demo",
                Email = "demo@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("demo1234"),
                CreatedAt = DateTime.UtcNow
            };

            db.Users.Add(demoUser);
            await db.SaveChangesAsync();

            var project = new Project
            {
                Title = "Demo Project",
                Description = "A sample project to get you started",
                CreatedAt = DateTime.UtcNow,
                UserId = demoUser.Id
            };
            db.Projects.Add(project);
            await db.SaveChangesAsync();

            db.TaskItems.AddRange(
                new TaskItem { ProjectId = project.Id, Title = "Try logging in as demo/demo1234", CreatedAt = DateTime.UtcNow },
                new TaskItem { ProjectId = project.Id, Title = "Create a new project", CreatedAt = DateTime.UtcNow },
                new TaskItem { ProjectId = project.Id, Title = "Add tasks to it", CreatedAt = DateTime.UtcNow }
            );
            await db.SaveChangesAsync();
        }
    }
}


