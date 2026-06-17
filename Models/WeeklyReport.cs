namespace AzubiLog.Models;

public class WeeklyReport
{
    public int Id { get; set; }
    public int Year { get; set; }
    public int CalendarWeek { get; set; }
    public string Status { get; set; } = "Offen"; // Offen, Eingereicht, Genehmigt, Abgelehnt
    public string? Comment { get; set; }
    public string? TrainerComment { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public List<ReportEntry> Entries { get; set; } = new();
}
