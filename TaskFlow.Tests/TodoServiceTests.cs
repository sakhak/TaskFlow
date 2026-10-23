using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Dtos;
using TaskFlow.Api.Models;
using TaskFlow.Api.Services;

namespace TaskFlow.Tests;

public class TodoServiceTests
{
    private static TodoService CreateService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TodoService(new AppDbContext(options));
    }

    [Fact]
    public async Task Create_TrimsTitle_AndDefaultsToTodo()
    {
        var service = CreateService();

        var result = await service.CreateAsync(new CreateTodoRequest { Title = "  Learn EF Core  " });

        Assert.Equal("Learn EF Core", result.Title);
        Assert.Equal("Todo", result.Status);
    }

    [Fact]
    public async Task GetAll_FiltersByStatus()
    {
        var service = CreateService();
        var a = await service.CreateAsync(new CreateTodoRequest { Title = "A" });
        await service.CreateAsync(new CreateTodoRequest { Title = "B" });
        await service.UpdateAsync(a.Id, new UpdateTodoRequest { Title = "A", Status = TodoStatus.Done });

        var done = await service.GetAllAsync(TodoStatus.Done, null, 1, 10);

        Assert.Single(done.Items);
        Assert.Equal("A", done.Items[0].Title);
    }

    [Fact]
    public async Task GetAll_SearchIsCaseInsensitive()
    {
        var service = CreateService();
        await service.CreateAsync(new CreateTodoRequest { Title = "Write Unit Tests" });
        await service.CreateAsync(new CreateTodoRequest { Title = "Buy milk" });

        var result = await service.GetAllAsync(null, "unit", 1, 10);

        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetAll_PagesResults()
    {
        var service = CreateService();
        for (var i = 1; i <= 5; i++)
            await service.CreateAsync(new CreateTodoRequest { Title = $"Task {i}" });

        var page2 = await service.GetAllAsync(null, null, 2, 2);

        Assert.Equal(2, page2.Items.Count);
        Assert.Equal(5, page2.TotalCount);
        Assert.Equal(3, page2.TotalPages);
    }

    [Fact]
    public async Task Update_ReturnsNull_WhenNotFound()
    {
        var service = CreateService();

        var result = await service.UpdateAsync(999, new UpdateTodoRequest { Title = "X" });

        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_RemovesItem()
    {
        var service = CreateService();
        var created = await service.CreateAsync(new CreateTodoRequest { Title = "Temp" });

        Assert.True(await service.DeleteAsync(created.Id));
        Assert.Null(await service.GetByIdAsync(created.Id));
        Assert.False(await service.DeleteAsync(created.Id));
    }
}
