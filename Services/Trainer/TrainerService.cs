using AzubiLog.Data;
using AzubiLog.Models;
using AzubiLog.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AzubiLog.Services.Trainer;

public sealed class TrainerService(
    ApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    UserManager<ApplicationUser> userManager) : ITrainerService
{
    public async Task<List<ApprenticeOverview>> GetAssignedApprenticesAsync(CancellationToken ct = default)
    {
        var trainer = await currentUserService.GetRequiredUserAsync(ct);

        var assignments = await dbContext.TrainerAssignments
            .Where(a => a.TrainerId == trainer.Id)
            .Include(a => a.Apprentice)
            .ToListAsync(ct);

        var result = new List<ApprenticeOverview>();
        foreach (var a in assignments)
        {
            var pending = await dbContext.WeeklyReports
                .CountAsync(w => w.UserId == a.ApprenticeId && w.Status == "Eingereicht", ct);
            var total = await dbContext.WeeklyReports
                .CountAsync(w => w.UserId == a.ApprenticeId, ct);

            result.Add(new ApprenticeOverview
            {
                UserId = a.ApprenticeId,
                FullName = $"{a.Apprentice.FirstName} {a.Apprentice.LastName}",
                Email = a.Apprentice.Email ?? string.Empty,
                School = a.Apprentice.School,
                ClassName = a.Apprentice.ClassName,
                PendingReports = pending,
                TotalReports = total,
            });
        }

        return result;
    }

    public async Task<List<WeeklyReport>> GetApprenticeReportsAsync(string apprenticeId, CancellationToken ct = default)
    {
        var trainer = await currentUserService.GetRequiredUserAsync(ct);
        var assigned = await dbContext.TrainerAssignments
            .AnyAsync(a => a.TrainerId == trainer.Id && a.ApprenticeId == apprenticeId, ct);

        if (!assigned) throw new UnauthorizedAccessException("Kein Zugriff auf diesen Azubi.");

        return await dbContext.WeeklyReports
            .Where(w => w.UserId == apprenticeId)
            .Include(w => w.Entries)
            .OrderByDescending(w => w.Year)
            .ThenByDescending(w => w.CalendarWeek)
            .ToListAsync(ct);
    }

    public async Task ReviewReportAsync(int weeklyReportId, string status, string? comment, CancellationToken ct = default)
    {
        var trainer = await currentUserService.GetRequiredUserAsync(ct);
        var report = await dbContext.WeeklyReports
            .FirstOrDefaultAsync(w => w.Id == weeklyReportId, ct)
            ?? throw new InvalidOperationException("Bericht nicht gefunden.");

        var assigned = await dbContext.TrainerAssignments
            .AnyAsync(a => a.TrainerId == trainer.Id && a.ApprenticeId == report.UserId, ct);
        if (!assigned) throw new UnauthorizedAccessException("Kein Zugriff.");

        report.Status = status; // Genehmigt or Abgelehnt
        report.TrainerComment = comment;
        report.ReviewedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task AssignApprenticeAsync(string apprenticeEmail, CancellationToken ct = default)
    {
        var trainer = await currentUserService.GetRequiredUserAsync(ct);
        var apprentice = await userManager.FindByEmailAsync(apprenticeEmail)
            ?? throw new InvalidOperationException("Azubi nicht gefunden.");

        var exists = await dbContext.TrainerAssignments
            .AnyAsync(a => a.TrainerId == trainer.Id && a.ApprenticeId == apprentice.Id, ct);
        if (exists) return;

        dbContext.TrainerAssignments.Add(new TrainerAssignment
        {
            TrainerId = trainer.Id,
            ApprenticeId = apprentice.Id,
        });
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task UnassignApprenticeAsync(string apprenticeId, CancellationToken ct = default)
    {
        var trainer = await currentUserService.GetRequiredUserAsync(ct);
        var assignment = await dbContext.TrainerAssignments
            .FirstOrDefaultAsync(a => a.TrainerId == trainer.Id && a.ApprenticeId == apprenticeId, ct);
        if (assignment is not null)
        {
            dbContext.TrainerAssignments.Remove(assignment);
            await dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<List<TodoItem>> GetApprenticeTodosAsync(string apprenticeId, CancellationToken ct = default)
    {
        var trainer = await currentUserService.GetRequiredUserAsync(ct);
        var assigned = await dbContext.TrainerAssignments
            .AnyAsync(a => a.TrainerId == trainer.Id && a.ApprenticeId == apprenticeId, ct);
        if (!assigned) throw new UnauthorizedAccessException("Kein Zugriff auf diesen Azubi.");

        return await dbContext.Todos
            .Where(t => t.UserId == apprenticeId && t.IsCompleted)
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<List<TodoItem>> GetAllApprenticeTodosAsync(CancellationToken ct = default)
    {
        var trainer = await currentUserService.GetRequiredUserAsync(ct);
        var apprenticeIds = await dbContext.TrainerAssignments
            .Where(a => a.TrainerId == trainer.Id)
            .Select(a => a.ApprenticeId)
            .ToListAsync(ct);

        return await dbContext.Todos
            .Where(t => apprenticeIds.Contains(t.UserId) && t.IsCompleted)
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task ReviewTodoAsync(int todoId, string status, string? comment, CancellationToken ct = default)
    {
        var trainer = await currentUserService.GetRequiredUserAsync(ct);
        var todo = await dbContext.Todos
            .FirstOrDefaultAsync(t => t.Id == todoId, ct)
            ?? throw new InvalidOperationException("Aufgabe nicht gefunden.");

        var assigned = await dbContext.TrainerAssignments
            .AnyAsync(a => a.TrainerId == trainer.Id && a.ApprenticeId == todo.UserId, ct);
        if (!assigned) throw new UnauthorizedAccessException("Kein Zugriff.");

        todo.ReviewStatus = status;
        todo.ReviewComment = comment;
        todo.ReviewedAt = DateTime.UtcNow;
        todo.ReviewedByUserId = trainer.Id;
        await dbContext.SaveChangesAsync(ct);
    }
}
