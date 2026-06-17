using System.Globalization;
using AzubiLog.Data;
using AzubiLog.Models;
using AzubiLog.Services.Identity;
using Microsoft.EntityFrameworkCore;

namespace AzubiLog.Services.WeeklyReports;

public sealed class WeeklyReportService(
    ApplicationDbContext dbContext,
    ICurrentUserService currentUserService) : IWeeklyReportService
{
    public async Task<List<WeeklyReport>> GetReportsAsync(CancellationToken ct = default)
    {
        var user = await currentUserService.GetRequiredUserAsync(ct);
        return await dbContext.WeeklyReports
            .Where(w => w.UserId == user.Id)
            .OrderByDescending(w => w.Year)
            .ThenByDescending(w => w.CalendarWeek)
            .ToListAsync(ct);
    }

    public async Task<WeeklyReport?> GetReportAsync(int year, int week, CancellationToken ct = default)
    {
        var user = await currentUserService.GetRequiredUserAsync(ct);
        return await dbContext.WeeklyReports
            .Include(w => w.Entries)
            .FirstOrDefaultAsync(w => w.UserId == user.Id && w.Year == year && w.CalendarWeek == week, ct);
    }

    public async Task<WeeklyReport> EnsureReportAsync(int year, int week, CancellationToken ct = default)
    {
        var user = await currentUserService.GetRequiredUserAsync(ct);
        var existing = await dbContext.WeeklyReports
            .FirstOrDefaultAsync(w => w.UserId == user.Id && w.Year == year && w.CalendarWeek == week, ct);

        if (existing is not null) return existing;

        var report = new WeeklyReport
        {
            Year = year,
            CalendarWeek = week,
            UserId = user.Id,
        };
        dbContext.WeeklyReports.Add(report);

        var weekStart = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
        var weekEnd = weekStart.AddDays(5);
        var entries = await dbContext.ReportEntries
            .Where(e => e.UserId == user.Id && e.Date >= weekStart && e.Date < weekEnd && e.WeeklyReportId == null)
            .ToListAsync(ct);

        foreach (var entry in entries)
        {
            entry.WeeklyReportId = report.Id;
        }

        await dbContext.SaveChangesAsync(ct);

        entries.ForEach(e => e.WeeklyReportId = report.Id);
        await dbContext.SaveChangesAsync(ct);

        report.Entries = entries;
        return report;
    }

    public async Task SubmitReportAsync(int id, CancellationToken ct = default)
    {
        var user = await currentUserService.GetRequiredUserAsync(ct);
        var report = await dbContext.WeeklyReports
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == user.Id, ct)
            ?? throw new InvalidOperationException("Bericht nicht gefunden.");

        report.Status = "Eingereicht";
        report.SubmittedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(ct);
    }
}
