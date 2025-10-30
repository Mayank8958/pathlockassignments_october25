using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Api.DTOs;
using ProjectManager.Api.Services;
using System.Security.Claims;

namespace ProjectManager.Api.Controllers;

[ApiController]
[Route("api/v1/projects/{projectId:int}/schedule")]
[Authorize]
public class ScheduleController : ControllerBase
{
    private readonly ISchedulerService _scheduler;
    public ScheduleController(ISchedulerService scheduler) { _scheduler = scheduler; }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpPost]
    public async Task<ActionResult<ScheduleResponse>> Create(int projectId, [FromBody] ScheduleRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var userId = GetUserId();
        try
        {
            var result = await _scheduler.CreateScheduleAsync(userId, projectId, request);
            if (result == null) return NotFound();
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}


