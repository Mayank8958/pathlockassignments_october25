using Microsoft.EntityFrameworkCore;
using ProjectManager.Api.Data;
using ProjectManager.Api.Models;

namespace ProjectManager.Api.Repositories;

public interface ITaskItemRepository
{
    Task<List<TaskItem>> GetByProjectAsync(int projectId);
    Task<TaskItem?> GetByIdAsync(int taskId);
    Task AddAsync(TaskItem task);
    Task SaveChangesAsync();
    void Remove(TaskItem task);
}

public class TaskItemRepository : ITaskItemRepository
{
    private readonly AppDbContext _db;
    public TaskItemRepository(AppDbContext db) { _db = db; }

    public Task<List<TaskItem>> GetByProjectAsync(int projectId) => _db.TaskItems.Where(t => t.ProjectId == projectId).OrderByDescending(t => t.CreatedAt).ToListAsync();
    public Task<TaskItem?> GetByIdAsync(int taskId) => _db.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId);
    public async Task AddAsync(TaskItem task) { await _db.TaskItems.AddAsync(task); }
    public Task SaveChangesAsync() => _db.SaveChangesAsync();
    public void Remove(TaskItem task) { _db.TaskItems.Remove(task); }
}


