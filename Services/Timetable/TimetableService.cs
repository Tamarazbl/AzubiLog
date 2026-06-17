using AzubiLog.Data;
using AzubiLog.Models;
using AzubiLog.Services.Identity;
using Microsoft.EntityFrameworkCore;

namespace AzubiLog.Services.Timetable;

public sealed class TimetableService(
    ApplicationDbContext dbContext,
    ICurrentUserService currentUserService) : ITimetableService
{
    public async Task<List<TimetableEntry>> GetTimetableAsync(string school, string className, CancellationToken ct = default)
    {
        var schoolLower = school.Trim().ToLowerInvariant();
        var classLower = className.Trim().ToLowerInvariant();
        return await dbContext.TimetableEntries
            .Where(t => t.School.ToLower() == schoolLower && t.ClassName.ToLower() == classLower)
            .OrderBy(t => t.DayOfWeek)
            .ThenBy(t => t.Period)
            .ToListAsync(ct);
    }

    public async Task<TimetableEntry> UpsertEntryAsync(TimetableEntry entry, CancellationToken ct = default)
    {
        var user = await currentUserService.GetRequiredUserAsync(ct);
        entry.CreatedByUserId = user.Id;

        var existing = await dbContext.TimetableEntries
            .FirstOrDefaultAsync(t =>
                t.School == entry.School
                && t.ClassName == entry.ClassName
                && t.DayOfWeek == entry.DayOfWeek
                && t.Period == entry.Period, ct);

        if (existing is not null)
        {
            existing.Subject = entry.Subject;
            existing.Teacher = entry.Teacher;
            existing.Room = entry.Room;
            existing.StartTime = entry.StartTime;
            existing.EndTime = entry.EndTime;
            existing.IsCancelled = entry.IsCancelled;
            existing.CreatedByUserId = user.Id;
            await dbContext.SaveChangesAsync(ct);
            return existing;
        }

        dbContext.TimetableEntries.Add(entry);
        await dbContext.SaveChangesAsync(ct);
        return entry;
    }

    public async Task DeleteEntryAsync(int id, CancellationToken ct = default)
    {
        var entry = await dbContext.TimetableEntries.FindAsync([id], ct);
        if (entry is not null)
        {
            dbContext.TimetableEntries.Remove(entry);
            await dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task ToggleCancelledAsync(int id, CancellationToken ct = default)
    {
        var entry = await dbContext.TimetableEntries.FindAsync([id], ct);
        if (entry is not null)
        {
            entry.IsCancelled = !entry.IsCancelled;
            await dbContext.SaveChangesAsync(ct);
        }
    }
}
