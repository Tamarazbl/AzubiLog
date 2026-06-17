namespace AzubiLog.Models;

public class ReportEntry
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string DayType { get; set; } = "Betrieb"; // Betrieb, Berufsschule, Feiertag, Urlaub, Krank
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string? Subject { get; set; } // Schulfach (only for Berufsschule)
    public string? OrderNumber { get; set; }
    public double Duration { get; set; }
    public string Status { get; set; } = "Entwurf"; // Entwurf, Eingereicht, Genehmigt, Abgelehnt

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int? WeeklyReportId { get; set; }
    public WeeklyReport? WeeklyReport { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}
