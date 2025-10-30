using TaskManager.DTOs;
using TaskManager.Models;

namespace TaskManager.Services;

public interface ITaskService
{
    IEnumerable<TaskItem> GetAll();
    TaskItem? GetById(int id);
    TaskItem Create(CreateTaskDto dto);
    bool Update(int id, UpdateTaskDto dto);
    bool Delete(int id);
}


