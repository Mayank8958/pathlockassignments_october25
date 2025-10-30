using ProjectManager.Api.DTOs;
using ProjectManager.Api.Models;
using ProjectManager.Api.Repositories;

namespace ProjectManager.Api.Services;

public interface ITaskItemService
{
    Task<List<TaskResponse>> GetTasksAsync(int userId, int projectId);
    Task<TaskResponse> CreateTaskAsync(int userId, int projectId, TaskCreateRequest request);
    Task<TaskResponse?> UpdateTaskAsync(int userId, int projectId, int taskId, TaskUpdateRequest request);
    Task<bool> ToggleTaskAsync(int userId, int projectId, int taskId);
    Task DeleteTaskAsync(int userId, int projectId, int taskId);
}

public class TaskItemService : ITaskItemService
{
    private readonly IProjectRepository _projects;
    private readonly ITaskItemRepository _tasks;

    public TaskItemService(IProjectRepository projects, ITaskItemRepository tasks)
    {
        _projects = projects;
        _tasks = tasks;
    }

    public async Task<List<TaskResponse>> GetTasksAsync(int userId, int projectId)
    {
        var project = await _projects.GetByIdAsync(projectId);
        if (project == null || project.UserId != userId) throw new UnauthorizedAccessException("Not your project");
        var list = await _tasks.GetByProjectAsync(projectId);
        return list.Select(Map).ToList();
    }

    public async Task<TaskResponse> CreateTaskAsync(int userId, int projectId, TaskCreateRequest request)
    {
        var project = await _projects.GetByIdAsync(projectId);
        if (project == null || project.UserId != userId) throw new UnauthorizedAccessException("Not your project");
        var task = new TaskItem
        {
            ProjectId = projectId,
            Title = request.Title,
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow
        };
        await _tasks.AddAsync(task);
        await _tasks.SaveChangesAsync();
        return Map(task);
    }

    public async Task<TaskResponse?> UpdateTaskAsync(int userId, int projectId, int taskId, TaskUpdateRequest request)
    {
        var project = await _projects.GetByIdAsync(projectId);
        if (project == null || project.UserId != userId) throw new UnauthorizedAccessException("Not your project");
        var task = await _tasks.GetByIdAsync(taskId);
        if (task == null || task.ProjectId != projectId) return null;
        task.Title = request.Title;
        task.DueDate = request.DueDate;
        task.IsCompleted = request.IsCompleted;
        await _tasks.SaveChangesAsync();
        return Map(task);
    }

    public async Task<bool> ToggleTaskAsync(int userId, int projectId, int taskId)
    {
        var project = await _projects.GetByIdAsync(projectId);
        if (project == null || project.UserId != userId) throw new UnauthorizedAccessException("Not your project");
        var task = await _tasks.GetByIdAsync(taskId);
        if (task == null || task.ProjectId != projectId) return false;
        task.IsCompleted = !task.IsCompleted;
        await _tasks.SaveChangesAsync();
        return task.IsCompleted;
    }

    public async Task DeleteTaskAsync(int userId, int projectId, int taskId)
    {
        var project = await _projects.GetByIdAsync(projectId);
        if (project == null || project.UserId != userId) throw new UnauthorizedAccessException("Not your project");
        var task = await _tasks.GetByIdAsync(taskId) ?? throw new KeyNotFoundException("Task not found");
        if (task.ProjectId != projectId) throw new KeyNotFoundException("Task not in this project");
        _tasks.Remove(task);
        await _tasks.SaveChangesAsync();
    }

    private static TaskResponse Map(TaskItem t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        DueDate = t.DueDate,
        IsCompleted = t.IsCompleted,
        CreatedAt = t.CreatedAt
    };
}


