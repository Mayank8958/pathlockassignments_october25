using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Api.Models;

public class Project
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


