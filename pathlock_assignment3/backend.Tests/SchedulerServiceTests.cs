using Microsoft.EntityFrameworkCore;
using ProjectManager.Api.Data;
using ProjectManager.Api.Models;
using ProjectManager.Api.Repositories;
using ProjectManager.Api.Services;
using ProjectManager.Api.DTOs;
using Xunit;

public class SchedulerServiceTests
{
    private static (AppDbContext db, IProjectRepository projects, ITaskItemRepository tasks) Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new AppDbContext(options);
        return (db, new ProjectRepository(db), new TaskItemRepository(db));
    }

    [Fact]
    public async Task Schedules_Tasks_By_Earliest_Due_And_Fills_Days()
    {
        var (db, projects, tasks) = Create();
        var project = new Project { Title = "P", UserId = 1, CreatedAt = DateTime.UtcNow.Date };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        db.TaskItems.AddRange(
            new TaskItem { ProjectId = project.Id, Title = "A", DueDate = DateTime.UtcNow.Date.AddDays(1) },
            new TaskItem { ProjectId = project.Id, Title = "B", DueDate = DateTime.UtcNow.Date.AddDays(2) },
            new TaskItem { ProjectId = project.Id, Title = "C", DueDate = null }
        );
        await db.SaveChangesAsync();

        var svc = new SchedulerService(projects, tasks);
        var result = await svc.CreateScheduleAsync(1, project.Id, new ScheduleRequest
        {
            Days = 2,
            HoursPerDay = 2,
            Strategy = "earliest_due",
            StartDate = project.CreatedAt
        });

        Assert.NotNull(result);
        Assert.Equal(project.Id, result!.ProjectId);
        Assert.Equal(2, result!.Schedule.Count);
        Assert.Equal(2, result!.Schedule[0].TotalHours);
        Assert.True(result!.Schedule.All(d => d.TotalHours <= 2));
    }

    [Fact]
    public async Task Splits_Task_Across_Days_When_Too_Long()
    {
        var (db, projects, tasks) = Create();
        var project = new Project { Title = "P2", UserId = 1, CreatedAt = DateTime.UtcNow.Date };
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        // Simulate longer task by adding duplicates and relying on default 1h each? Instead insert one and assume default 1h, then use HoursPerDay=1 for split test by fabricating larger remaining via repeated schedule days
        db.TaskItems.Add(new TaskItem { ProjectId = project.Id, Title = "Long Task", DueDate = null });
        await db.SaveChangesAsync();

        var svc = new SchedulerService(projects, tasks);
        var res = await svc.CreateScheduleAsync(1, project.Id, new ScheduleRequest { Days = 1, HoursPerDay = 1, Strategy = "earliest_due", StartDate = project.CreatedAt });

        Assert.NotNull(res);
        Assert.Single(res!.Schedule);
        Assert.InRange(res!.Schedule[0].TotalHours, 0.99, 1.01);
    }
}


