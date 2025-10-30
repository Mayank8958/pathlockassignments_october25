using Microsoft.EntityFrameworkCore;
using ProjectManager.Api.Data;
using ProjectManager.Api.Models;

namespace ProjectManager.Api.Repositories;

public interface IProjectRepository
{
    Task<List<Project>> GetByUserAsync(int userId);
    Task<Project?> GetByIdAsync(int projectId);
    Task AddAsync(Project project);
    void Remove(Project project);
    Task SaveChangesAsync();
}

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _db;
    public ProjectRepository(AppDbContext db) { _db = db; }

    public Task<List<Project>> GetByUserAsync(int userId) => _db.Projects.Where(p => p.UserId == userId).OrderByDescending(p => p.CreatedAt).ToListAsync();
    public Task<Project?> GetByIdAsync(int projectId) => _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
    public async Task AddAsync(Project project) { await _db.Projects.AddAsync(project); }
    public void Remove(Project project) { _db.Projects.Remove(project); }
    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}


