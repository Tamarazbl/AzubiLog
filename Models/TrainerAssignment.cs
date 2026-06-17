namespace AzubiLog.Models;

public class TrainerAssignment
{
    public int Id { get; set; }

    public string TrainerId { get; set; } = string.Empty;
    public ApplicationUser Trainer { get; set; } = null!;

    public string ApprenticeId { get; set; } = string.Empty;
    public ApplicationUser Apprentice { get; set; } = null!;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
