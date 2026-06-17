namespace AzubiLog.Models;

public class TimetableEntry
{
    public int Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public int Period { get; set; } // 1, 2, 3...
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Teacher { get; set; }
    public string? Room { get; set; }
    public bool IsCancelled { get; set; }

    public string School { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;

    public string CreatedByUserId { get; set; } = string.Empty;
    public ApplicationUser CreatedBy { get; set; } = null!;
}
