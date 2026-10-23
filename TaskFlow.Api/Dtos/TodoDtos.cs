using System.ComponentModel.DataAnnotations;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Dtos;

public class CreateTodoRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }
}

public class UpdateTodoRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [EnumDataType(typeof(TodoStatus))]
    public TodoStatus Status { get; set; }

    public DateTime? DueDate { get; set; }
}

public record TodoResponse(
    int Id,
    string Title,
    string? Description,
    string Status,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
