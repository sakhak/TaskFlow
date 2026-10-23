using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Dtos;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services;

public class TodoService : ITodoService
{
    private readonly AppDbContext _db;

    public TodoService(AppDbContext db) => _db = db;

    public async Task<PagedResult<TodoResponse>> GetAllAsync(
        TodoStatus? status, string? search, int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _db.Todos.AsNoTracking().AsQueryable();

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(t => t.Title.ToLower().Contains(term));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<TodoResponse>(items.Select(ToResponse).ToList(), page, pageSize, total);
    }

    public async Task<TodoResponse?> GetByIdAsync(int id)
    {
        var todo = await _db.Todos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        return todo is null ? null : ToResponse(todo);
    }

    public async Task<TodoResponse> CreateAsync(CreateTodoRequest request)
    {
        var todo = new TodoItem
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            DueDate = request.DueDate
        };

        _db.Todos.Add(todo);
        await _db.SaveChangesAsync();
        return ToResponse(todo);
    }

    public async Task<TodoResponse?> UpdateAsync(int id, UpdateTodoRequest request)
    {
        var todo = await _db.Todos.FindAsync(id);
        if (todo is null) return null;

        todo.Title = request.Title.Trim();
        todo.Description = request.Description?.Trim();
        todo.Status = request.Status;
        todo.DueDate = request.DueDate;
        todo.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return ToResponse(todo);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var todo = await _db.Todos.FindAsync(id);
        if (todo is null) return false;

        _db.Todos.Remove(todo);
        await _db.SaveChangesAsync();
        return true;
    }

    private static TodoResponse ToResponse(TodoItem t) =>
        new(t.Id, t.Title, t.Description, t.Status.ToString(), t.DueDate, t.CreatedAt, t.UpdatedAt);
}
