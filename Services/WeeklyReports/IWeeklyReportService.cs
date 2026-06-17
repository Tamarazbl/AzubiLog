using AzubiLog.Models;

namespace AzubiLog.Services.WeeklyReports;

public interface IWeeklyReportService
{
    Task<List<WeeklyReport>> GetReportsAsync(CancellationToken ct = default);
    Task<WeeklyReport?> GetReportAsync(int year, int week, CancellationToken ct = default);
    Task<WeeklyReport> EnsureReportAsync(int year, int week, CancellationToken ct = default);
    Task SubmitReportAsync(int id, CancellationToken ct = default);
}
