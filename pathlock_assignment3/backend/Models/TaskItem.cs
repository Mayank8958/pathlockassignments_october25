using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Api.Models;

public class TaskItem
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


