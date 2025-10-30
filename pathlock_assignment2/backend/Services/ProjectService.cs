using ProjectManager.Api.DTOs;
using ProjectManager.Api.Models;
using ProjectManager.Api.Repositories;

namespace ProjectManager.Api.Services;

public interface IProjectService
{
    Task<List<ProjectResponse>> GetProjectsAsync(int userId);
    Task<ProjectResponse?> GetProjectAsync(int userId, int projectId);
    Task<ProjectResponse> CreateProjectAsync(int userId, ProjectCreateRequest request);
    Task DeleteProjectAsync(int userId, int projectId);
}

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projects;

    public ProjectService(IProjectRepository projects)
    {
        _projects = projects;
    }

    public async Task<List<ProjectResponse>> GetProjectsAsync(int userId)
    {
        var list = await _projects.GetByUserAsync(userId);
        return list.Select(Map).ToList();
    }

    public async Task<ProjectResponse?> GetProjectAsync(int userId, int projectId)
    {
        var project = await _projects.GetByIdAsync(projectId);
        if (project == null || project.UserId != userId) return null;
        return Map(project);
    }

    public async Task<ProjectResponse> CreateProjectAsync(int userId, ProjectCreateRequest request)
    {
        var project = new Project
        {
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };
        await _projects.AddAsync(project);
        await _projects.SaveChangesAsync();
        return Map(project);
    }

    public async Task DeleteProjectAsync(int userId, int projectId)
    {
        var project = await _projects.GetByIdAsync(projectId) ?? throw new KeyNotFoundException("Project not found");
        if (project.UserId != userId) throw new UnauthorizedAccessException("Not your project");
        _projects.Remove(project);
        await _projects.SaveChangesAsync();
    }

    private static ProjectResponse Map(Project p) => new()
    {
        Id = p.Id,
        Title = p.Title,
        Description = p.Description,
        CreatedAt = p.CreatedAt
    };
}


