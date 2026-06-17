using AzubiLog.Models;

namespace AzubiLog.Services.ReportEntries;

public interface IReportEntryService
{
    Task<List<ReportEntry>> GetEntriesForDateAsync(DateTime date, CancellationToken ct = default);
    Task<ReportEntry?> GetEntryByIdAsync(int id, CancellationToken ct = default);
    Task<ReportEntry> CreateEntryAsync(ReportEntry entry, CancellationToken ct = default);
    Task UpdateEntryAsync(ReportEntry entry, CancellationToken ct = default);
    Task DeleteEntryAsync(int id, CancellationToken ct = default);
    Task<List<DailySummary>> GetWeekSummaryAsync(DateTime weekStart, CancellationToken ct = default);
}

public sealed class DailySummary
{
    public DateTime Date { get; set; }
    public string DayType { get; set; } = string.Empty;
    public int EntryCount { get; set; }
    public double TotalHours { get; set; }
}
