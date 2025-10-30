using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Api.DTOs;

public class TaskCreateRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
}

public class TaskUpdateRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
}

public class TaskResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
}


