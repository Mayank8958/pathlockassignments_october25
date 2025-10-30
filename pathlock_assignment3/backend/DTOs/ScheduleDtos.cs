using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Api.DTOs;

public class ScheduleRequest
{
    [Range(1, 365)]
    public int Days { get; set; } = 5;

    [Range(1, 24)]
    public int HoursPerDay { get; set; } = 4;

    public DateTime? StartDate { get; set; }

    // earliest_due, longest_task, priority (future)
    [Required]
    public string Strategy { get; set; } = "earliest_due";
}

public class ScheduledTaskEntry
{
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public double EstimatedHours { get; set; }
}

public class ScheduleDay
{
    public DateTime Date { get; set; }
    public List<ScheduledTaskEntry> Tasks { get; set; } = new();
    public double TotalHours { get; set; }
}

public class ScheduleResponse
{
    public int ProjectId { get; set; }
    public DateTime StartDate { get; set; }
    public List<ScheduleDay> Schedule { get; set; } = new();
    public List<ScheduledTaskEntry>? Overflow { get; set; }
}


