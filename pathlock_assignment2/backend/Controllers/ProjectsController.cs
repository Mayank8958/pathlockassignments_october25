using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Api.DTOs;
using ProjectManager.Api.Services;
using System.Security.Claims;

namespace ProjectManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projects;
    public ProjectsController(IProjectService projects) { _projects = projects; }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpGet]
    public async Task<ActionResult<List<ProjectResponse>>> Get()
    {
        var userId = GetUserId();
        var list = await _projects.GetProjectsAsync(userId);
        return Ok(list);
    }

    [HttpGet("{projectId:int}")]
    public async Task<ActionResult<ProjectResponse>> GetOne(int projectId)
    {
        var userId = GetUserId();
        var project = await _projects.GetProjectAsync(userId, projectId);
        if (project == null) return NotFound();
        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create([FromBody] ProjectCreateRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var userId = GetUserId();
        var created = await _projects.CreateProjectAsync(userId, request);
        return CreatedAtAction(nameof(GetOne), new { projectId = created.Id }, created);
    }

    [HttpDelete("{projectId:int}")]
    public async Task<IActionResult> Delete(int projectId)
    {
        var userId = GetUserId();
        try
        {
            await _projects.DeleteProjectAsync(userId, projectId);
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


