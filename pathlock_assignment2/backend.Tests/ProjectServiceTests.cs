using Microsoft.EntityFrameworkCore;
using ProjectManager.Api.Data;
using ProjectManager.Api.DTOs;
using ProjectManager.Api.Models;
using ProjectManager.Api.Repositories;
using ProjectManager.Api.Services;
using Xunit;

public class ProjectServiceTests
{
    private static (AppDbContext db, IProjectRepository projects) Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new AppDbContext(options);
        return (db, new ProjectRepository(db));
    }

    [Fact]
    public async Task Create_And_Get_Project_Works()
    {
        var (db, repo) = Create();
        var service = new ProjectService(repo);
        var userId = 1;

        var created = await service.CreateProjectAsync(userId, new ProjectCreateRequest { Title = "My Project" });
        Assert.True(created.Id > 0);

        var list = await service.GetProjectsAsync(userId);
        Assert.Single(list);

        var detail = await service.GetProjectAsync(userId, created.Id);
        Assert.NotNull(detail);
        Assert.Equal("My Project", detail!.Title);
    }

    [Fact]
    public async Task Delete_Project_Enforces_Ownership()
    {
        var (db, repo) = Create();
        var svc = new ProjectService(repo);
        var p = new Project { Title = "X", UserId = 1, CreatedAt = DateTime.UtcNow };
        db.Projects.Add(p);
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => svc.DeleteProjectAsync(2, p.Id));
    }
}


