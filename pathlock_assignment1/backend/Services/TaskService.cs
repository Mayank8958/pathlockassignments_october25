using System.ComponentModel.DataAnnotations;
using TaskManager.DTOs;
using TaskManager.Models;

namespace TaskManager.Services;

public class TaskService : ITaskService
{
    private readonly List<TaskItem> _tasks = new();
    private int _nextId = 1;

    public IEnumerable<TaskItem> GetAll() => _tasks.OrderByDescending(t => t.CreatedAt);

    public TaskItem? GetById(int id) => _tasks.FirstOrDefault(t => t.Id == id);

    public TaskItem Create(CreateTaskDto dto)
    {
        // DataAnnotations validation backup (controller validates too)
        Validator.ValidateObject(dto, new ValidationContext(dto), validateAllProperties: true);

        var task = new TaskItem
        {
            Id = _nextId++,
            Title = dto.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };
        _tasks.Add(task);
        return task;
    }

    public bool Update(int id, UpdateTaskDto dto)
    {
        var existing = GetById(id);
        if (existing is null) return false;

        existing.Title = dto.Title.Trim();
        existing.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description;
        existing.IsCompleted = dto.IsCompleted;
        return true;
    }

    public bool Delete(int id)
    {
        var existing = GetById(id);
        if (existing is null) return false;
        _tasks.Remove(existing);
        return true;
    }
}


