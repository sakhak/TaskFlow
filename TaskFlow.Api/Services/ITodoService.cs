using TaskFlow.Api.Dtos;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services;

public interface ITodoService
{
    Task<PagedResult<TodoResponse>> GetAllAsync(TodoStatus? status, string? search, int page, int pageSize);
    Task<TodoResponse?> GetByIdAsync(int id);
    Task<TodoResponse> CreateAsync(CreateTodoRequest request);
    Task<TodoResponse?> UpdateAsync(int id, UpdateTodoRequest request);
    Task<bool> DeleteAsync(int id);
}
