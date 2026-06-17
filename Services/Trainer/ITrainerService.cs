using AzubiLog.Models;

namespace AzubiLog.Services.Trainer;

public interface ITrainerService
{
    Task<List<ApprenticeOverview>> GetAssignedApprenticesAsync(CancellationToken ct = default);
    Task<List<WeeklyReport>> GetApprenticeReportsAsync(string apprenticeId, CancellationToken ct = default);
    Task ReviewReportAsync(int weeklyReportId, string status, string? comment, CancellationToken ct = default);
    Task AssignApprenticeAsync(string apprenticeEmail, CancellationToken ct = default);
    Task UnassignApprenticeAsync(string apprenticeId, CancellationToken ct = default);
    Task<List<TodoItem>> GetApprenticeTodosAsync(string apprenticeId, CancellationToken ct = default);
    Task<List<TodoItem>> GetAllApprenticeTodosAsync(CancellationToken ct = default);
    Task ReviewTodoAsync(int todoId, string status, string? comment, CancellationToken ct = default);
}

public sealed class ApprenticeOverview
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string School { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public int PendingReports { get; set; }
    public int TotalReports { get; set; }
}
