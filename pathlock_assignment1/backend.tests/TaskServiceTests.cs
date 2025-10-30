using TaskManager.DTOs;
using TaskManager.Services;
using Xunit;

namespace TaskManager.Tests;

public class TaskServiceTests
{
    [Fact]
    public void Create_Should_Add_Task_With_Auto_Id_And_Defaults()
    {
        var service = new TaskService();
        var created = service.Create(new CreateTaskDto { Title = "Test task" });

        Assert.Equal(1, created.Id);
        Assert.Equal("Test task", created.Title);
        Assert.False(created.IsCompleted);
        Assert.True((DateTime.UtcNow - created.CreatedAt).TotalSeconds < 5);
    }

    [Fact]
    public void Update_Should_Modify_Task_And_Return_True()
    {
        var service = new TaskService();
        var created = service.Create(new CreateTaskDto { Title = "Old" });

        var ok = service.Update(created.Id, new UpdateTaskDto
        {
            Title = "New",
            Description = "Desc",
            IsCompleted = true
        });

        Assert.True(ok);
        var fetched = service.GetById(created.Id)!;
        Assert.Equal("New", fetched.Title);
        Assert.Equal("Desc", fetched.Description);
        Assert.True(fetched.IsCompleted);
    }
}


