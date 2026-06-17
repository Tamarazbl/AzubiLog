using AzubiLog.Data;
using AzubiLog.Models;
using AzubiLog.Services.Identity;
using Microsoft.EntityFrameworkCore;

namespace AzubiLog.Services.ReportEntries;

public sealed class ReportEntryService(
    ApplicationDbContext dbContext,
    ICurrentUserService currentUserService) : IReportEntryService
{
    public async Task<List<ReportEntry>> GetEntriesForDateAsync(DateTime date, CancellationToken ct = default)
    {
        var user = await currentUserService.GetRequiredUserAsync(ct);
        return await dbContext.ReportEntries
            .Where(e => e.UserId == user.Id && e.Date.Date == date.Date)
            .OrderBy(e => e.Id)
            .ToListAsync(ct);
    }

    public async Task<ReportEntry?> GetEntryByIdAsync(int id, CancellationToken ct = default)
    {
        var user = await currentUserService.GetRequiredUserAsync(ct);
        return await dbContext.ReportEntries
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id, ct);
    }

    public async Task<ReportEntry> CreateEntryAsync(ReportEntry entry, CancellationToken ct = default)
    {
        var user = await currentUserService.GetRequiredUserAsync(ct);
        entry.UserId = user.Id;
        dbContext.ReportEntries.Add(entry);
        await dbContext.SaveChangesAsync(ct);
        return entry;
    }

    public async Task UpdateEntryAsync(ReportEntry entry, CancellationToken ct = default)
    {
        var user = await currentUserService.GetRequiredUserAsync(ct);
        var existing = await dbContext.ReportEntries
            .FirstOrDefaultAsync(e => e.Id == entry.Id && e.UserId == user.Id, ct)
            ?? throw new InvalidOperationException("Eintrag nicht gefunden.");

        existing.Date = entry.Date;
        existing.DayType = entry.DayType;
        existing.Title = entry.Title;
        existing.Description = entry.Description;
        existing.Note = entry.Note;
        existing.Subject = entry.Subject;
        existing.Duration = entry.Duration;
        existing.CategoryId = entry.CategoryId;
        existing.Status = entry.Status;

        await dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteEntryAsync(int id, CancellationToken ct = default)
    {
        var user = await currentUserService.GetRequiredUserAsync(ct);
        var entry = await dbContext.ReportEntries
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id, ct);
        if (entry is not null)
        {
            dbContext.ReportEntries.Remove(entry);
            await dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<List<DailySummary>> GetWeekSummaryAsync(DateTime weekStart, CancellationToken ct = default)
    {
        var user = await currentUserService.GetRequiredUserAsync(ct);
        var weekEnd = weekStart.AddDays(7);

        return await dbContext.ReportEntries
            .Where(e => e.UserId == user.Id && e.Date >= weekStart && e.Date < weekEnd)
            .GroupBy(e => e.Date.Date)
            .Select(g => new DailySummary
            {
                Date = g.Key,
                DayType = g.First().DayType,
                EntryCount = g.Count(),
                TotalHours = g.Sum(e => e.Duration),
            })
            .OrderBy(s => s.Date)
            .ToListAsync(ct);
    }
}
