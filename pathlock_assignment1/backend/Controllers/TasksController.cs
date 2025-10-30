using Microsoft.AspNetCore.Mvc;
using TaskManager.DTOs;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    private readonly ITaskService _taskService = taskService;

    [HttpGet]
    public ActionResult<IEnumerable<TaskDto>> GetAll()
    {
        var dtos = _taskService.GetAll().Select(ToDto);
        return Ok(dtos);
    }

    [HttpGet("{id:int}")]
    public ActionResult<TaskDto> GetById(int id)
    {
        var task = _taskService.GetById(id);
        if (task is null) return NotFound(new { message = "Task not found" });
        return Ok(ToDto(task));
    }

    [HttpPost]
    public ActionResult<TaskDto> Create([FromBody] CreateTaskDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var created = _taskService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] UpdateTaskDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var ok = _taskService.Update(id, dto);
        if (!ok) return NotFound(new { message = "Task not found" });
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var ok = _taskService.Delete(id);
        if (!ok) return NotFound(new { message = "Task not found" });
        return NoContent();
    }

    private static TaskDto ToDto(TaskItem t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        IsCompleted = t.IsCompleted,
        CreatedAt = t.CreatedAt
    };
}


