using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ProjectManager.Api.Controllers;
using ProjectManager.Api.Data;
using ProjectManager.Api.DTOs;
using ProjectManager.Api.Models;
using ProjectManager.Api.Repositories;
using ProjectManager.Api.Services;
using Xunit;

public class ScheduleEndpointTests
{
    private static (ScheduleController ctrl, Project project) Create(int userIdOwner)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new AppDbContext(options);
        var projects = new ProjectRepository(db);
        var tasks = new TaskItemRepository(db);
        var svc = new SchedulerService(projects, tasks);

        var p = new Project { Title = "P", UserId = userIdOwner, CreatedAt = DateTime.UtcNow.Date };
        db.Projects.Add(p);
        db.SaveChanges();

        var ctrl = new ScheduleController(svc);
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userIdOwner.ToString()) }, "TestAuth");
        httpContext.User = new ClaimsPrincipal(identity);
        ctrl.ControllerContext = new ControllerContext { HttpContext = httpContext };
        return (ctrl, p);
    }

    [Fact]
    public async Task Returns_404_When_Project_Not_Found()
    {
        var (ctrl, _) = Create(1);
        var result = await ctrl.Create(9999, new ScheduleRequest { Days = 1, HoursPerDay = 1, Strategy = "earliest_due" });
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Returns_403_When_Not_Owner()
    {
        var (ctrl, p) = Create(1);
        // Switch user to 2
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "2") }, "TestAuth");
        httpContext.User = new ClaimsPrincipal(identity);
        ctrl.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = await ctrl.Create(p.Id, new ScheduleRequest { Days = 1, HoursPerDay = 1, Strategy = "earliest_due" });
        Assert.IsType<ForbidResult>(result.Result);
    }
}


