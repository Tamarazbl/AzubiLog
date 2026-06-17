using AzubiLog.Models;

namespace AzubiLog.Services.Todos;

public interface ITodoService
{
    Task<List<TodoItem>> GetTodosAsync(CancellationToken ct = default);
    Task<TodoItem> CreateTodoAsync(string title, string? description, DateTime? dueDate, CancellationToken ct = default);
    Task ToggleTodoAsync(int id, CancellationToken ct = default);
    Task DeleteTodoAsync(int id, CancellationToken ct = default);
}
