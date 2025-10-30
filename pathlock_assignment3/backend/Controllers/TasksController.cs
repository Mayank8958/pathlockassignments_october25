using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Api.DTOs;
using ProjectManager.Api.Services;
using System.Security.Claims;

namespace ProjectManager.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:int}/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskItemService _tasks;
    public TasksController(ITaskItemService tasks) { _tasks = tasks; }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpGet]
    public async Task<ActionResult<List<TaskResponse>>> Get(int projectId)
    {
        var userId = GetUserId();
        try
        {
            var list = await _tasks.GetTasksAsync(userId, projectId);
            return Ok(list);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(int projectId, [FromBody] TaskCreateRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var userId = GetUserId();
        try
        {
            var created = await _tasks.CreateTaskAsync(userId, projectId, request);
            return CreatedAtAction(nameof(Get), new { projectId }, created);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPut("{taskId:int}")]
    public async Task<ActionResult<TaskResponse>> Update(int projectId, int taskId, [FromBody] TaskUpdateRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var userId = GetUserId();
        try
        {
            var updated = await _tasks.UpdateTaskAsync(userId, projectId, taskId, request);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPatch("{taskId:int}/toggle")]
    public async Task<ActionResult<object>> Toggle(int projectId, int taskId)
    {
        var userId = GetUserId();
        try
        {
            var state = await _tasks.ToggleTaskAsync(userId, projectId, taskId);
            if (!state && state != false) return NotFound();
            return Ok(new { isCompleted = state });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpDelete("{taskId:int}")]
    public async Task<IActionResult> Delete(int projectId, int taskId)
    {
        var userId = GetUserId();
        try
        {
            await _tasks.DeleteTaskAsync(userId, projectId, taskId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}


