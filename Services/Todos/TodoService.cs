using AzubiLog.Data;
using AzubiLog.Models;
using AzubiLog.Services.Identity;
using Microsoft.EntityFrameworkCore;

namespace AzubiLog.Services.Todos;

public sealed class TodoService(
    ApplicationDbContext dbContext,
    ICurrentUserService currentUserService) : ITodoService
{
    public async Task<List<TodoItem>> GetTodosAsync(CancellationToken ct = default)
    {
        var user = await currentUserService.GetRequiredUserAsync(ct);
        return await dbContext.Todos
            .Where(t => t.UserId == user.Id)
            .OrderBy(t => t.IsCompleted)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<TodoItem> CreateTodoAsync(string title, string? description, DateTime? dueDate, CancellationToken ct = default)
    {
        var user = await currentUserService.GetRequiredUserAsync(ct);
        var todo = new TodoItem
        {
            Title = title.Trim(),
            Description = description?.Trim(),
            DueDate = dueDate,
            UserId = user.Id,
        };
        dbContext.Todos.Add(todo);
        await dbContext.SaveChangesAsync(ct);
        return todo;
    }

    public async Task ToggleTodoAsync(int id, CancellationToken ct = default)
    {
        var user = await currentUserService.GetRequiredUserAsync(ct);
        var todo = await dbContext.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id, ct);
        if (todo is not null)
        {
            todo.IsCompleted = !todo.IsCompleted;
            await dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task DeleteTodoAsync(int id, CancellationToken ct = default)
    {
        var user = await currentUserService.GetRequiredUserAsync(ct);
        var todo = await dbContext.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id, ct);
        if (todo is not null)
        {
            dbContext.Todos.Remove(todo);
            await dbContext.SaveChangesAsync(ct);
        }
    }
}
