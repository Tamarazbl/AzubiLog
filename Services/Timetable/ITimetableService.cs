using AzubiLog.Models;

namespace AzubiLog.Services.Timetable;

public interface ITimetableService
{
    Task<List<TimetableEntry>> GetTimetableAsync(string school, string className, CancellationToken ct = default);
    Task<TimetableEntry> UpsertEntryAsync(TimetableEntry entry, CancellationToken ct = default);
    Task DeleteEntryAsync(int id, CancellationToken ct = default);
    Task ToggleCancelledAsync(int id, CancellationToken ct = default);
}
