using System.Globalization;
using AzubiLog.Data;
using AzubiLog.Services.Identity;
using Microsoft.EntityFrameworkCore;

namespace AzubiLog.Services.Dashboard;

public sealed class DashboardService(
    ApplicationDbContext dbContext,
    ICurrentUserService currentUserService) : IDashboardService
{
    public async Task<DashboardViewModel> GetDashboardAsync(CancellationToken ct = default)
    {
        var user = await currentUserService.GetRequiredUserAsync(ct);
        var today = DateTime.Today;
        var cw = ISOWeek.GetWeekOfYear(today);
        var weekStart = ISOWeek.ToDateTime(today.Year, cw, DayOfWeek.Monday);
        var weekEnd = weekStart.AddDays(7);

        var weekEntries = await dbContext.ReportEntries
            .Where(e => e.UserId == user.Id && e.Date >= weekStart && e.Date < weekEnd)
            .ToListAsync(ct);

        var recordedHours = weekEntries.Sum(e => e.Duration);

        var openTodos = await dbContext.Todos
            .CountAsync(t => t.UserId == user.Id && !t.IsCompleted, ct);

        var pendingReports = await dbContext.WeeklyReports
            .CountAsync(w => w.UserId == user.Id && w.Status == "Offen", ct);

        var recentEntries = await dbContext.ReportEntries
            .Where(e => e.UserId == user.Id)
            .OrderByDescending(e => e.Date)
            .ThenByDescending(e => e.Id)
            .Take(5)
            .Select(e => new RecentEntryViewModel
            {
                Id = e.Id,
                Date = e.Date,
                Title = e.Title,
                DayType = e.DayType,
                Duration = e.Duration,
            })
            .ToListAsync(ct);

        return new DashboardViewModel
        {
            CalendarWeek = cw,
            WeeklyTargetHours = user.WeeklyTargetHours,
            RecordedHours = recordedHours,
            RemainingHours = Math.Max(0, user.WeeklyTargetHours - recordedHours),
            OpenTodos = openTodos,
            PendingReports = pendingReports,
            UserFirstName = user.FirstName,
            RecentEntries = recentEntries,
        };
    }
}
