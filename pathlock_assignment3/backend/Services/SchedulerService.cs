using ProjectManager.Api.DTOs;
using ProjectManager.Api.Models;
using ProjectManager.Api.Repositories;

namespace ProjectManager.Api.Services;

public interface ISchedulerService
{
    Task<ScheduleResponse?> CreateScheduleAsync(int userId, int projectId, ScheduleRequest request);
}

public class SchedulerService : ISchedulerService
{
    private readonly IProjectRepository _projects;
    private readonly ITaskItemRepository _tasks;

    public SchedulerService(IProjectRepository projects, ITaskItemRepository tasks)
    {
        _projects = projects;
        _tasks = tasks;
    }

    public async Task<ScheduleResponse?> CreateScheduleAsync(int userId, int projectId, ScheduleRequest request)
    {
        var project = await _projects.GetByIdAsync(projectId);
        if (project == null) return null;
        if (project.UserId != userId) throw new UnauthorizedAccessException("Not your project");

        var allTasks = await _tasks.GetByProjectAsync(projectId);
        var pending = allTasks.Where(t => !t.IsCompleted).ToList();

        // Map to working set with derived estimated hours (default 1)
        var estimations = pending.Select(t => new WorkingTask
        {
            TaskId = t.Id,
            Title = t.Title,
            DueDate = t.DueDate,
            EstimatedHours = 1.0 // Extend in future if EstimatedHours exists
        }).ToList();

        // Sort by strategy
        var strategy = (request.Strategy ?? "earliest_due").Trim().ToLowerInvariant();
        estimations = strategy switch
        {
            "longest_task" => estimations
                .OrderByDescending(t => t.EstimatedHours)
                .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
                .ToList(),
            _ => estimations
                .OrderBy(t => t.DueDate ?? DateTime.MaxValue)
                .ThenByDescending(t => t.EstimatedHours)
                .ToList()
        };

        var startDate = (request.StartDate ?? project.CreatedAt.Date).Date;
        var hoursPerDay = Math.Max(1, Math.Min(24, request.HoursPerDay));
        var days = Math.Max(1, Math.Min(365, request.Days));

        var response = new ScheduleResponse
        {
            ProjectId = projectId,
            StartDate = startDate,
            Schedule = new List<ScheduleDay>()
        };

        // work queue with remaining hours
        var queue = new Queue<WorkingTask>(estimations.Select(t => t.Clone()));

        for (int i = 0; i < days; i++)
        {
            var date = startDate.AddDays(i);
            var day = new ScheduleDay { Date = date };
            double remaining = hoursPerDay;

            // keep filling from queue
            while (remaining > 0 && queue.Count > 0)
            {
                var current = queue.Peek();
                var take = Math.Min(remaining, current.RemainingHours);
                if (take <= 0.0001)
                {
                    queue.Dequeue();
                    continue;
                }

                day.Tasks.Add(new ScheduledTaskEntry
                {
                    TaskId = current.TaskId,
                    Title = current.Title,
                    EstimatedHours = take
                });
                day.TotalHours += take;
                remaining -= take;
                current.RemainingHours -= take;

                if (current.RemainingHours <= 0.0001)
                {
                    queue.Dequeue();
                }
            }

            response.Schedule.Add(day);
        }

        if (queue.Count > 0)
        {
            var overflow = new List<ScheduledTaskEntry>();
            while (queue.Count > 0)
            {
                var t = queue.Dequeue();
                overflow.Add(new ScheduledTaskEntry
                {
                    TaskId = t.TaskId,
                    Title = t.Title,
                    EstimatedHours = Math.Round(t.RemainingHours, 2)
                });
            }
            response.Overflow = overflow;
        }

        return response;
    }

    private class WorkingTask
    {
        public int TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public double EstimatedHours { get; set; }
        public double RemainingHours { get; set; }

        public WorkingTask Clone() => new()
        {
            TaskId = TaskId,
            Title = Title,
            DueDate = DueDate,
            EstimatedHours = EstimatedHours,
            RemainingHours = EstimatedHours
        };
    }
}


