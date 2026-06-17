namespace AzubiLog.Services.Dashboard;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync(CancellationToken ct = default);
}

public sealed class DashboardViewModel
{
    public int CalendarWeek { get; set; }
    public double WeeklyTargetHours { get; set; }
    public double RecordedHours { get; set; }
    public double RemainingHours { get; set; }
    public int OpenTodos { get; set; }
    public int PendingReports { get; set; }
    public string UserFirstName { get; set; } = string.Empty;
    public List<RecentEntryViewModel> RecentEntries { get; set; } = new();
}

public sealed class RecentEntryViewModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string DayType { get; set; } = string.Empty;
    public double Duration { get; set; }
}
