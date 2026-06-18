using System.ComponentModel.DataAnnotations;

namespace AzubiLog.Services.Account;

public sealed class AccountOverviewFormModel
{
    [Required(ErrorMessage = "Bitte gib deinen Vornamen ein.")]
    [StringLength(100, ErrorMessage = "Der Vorname darf maximal 100 Zeichen lang sein.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bitte gib deinen Nachnamen ein.")]
    [StringLength(100, ErrorMessage = "Der Nachname darf maximal 100 Zeichen lang sein.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bitte gib deine E-Mail-Adresse ein.")]
    [EmailAddress(ErrorMessage = "Bitte gib eine gültige E-Mail-Adresse ein.")]
    [StringLength(256, ErrorMessage = "Die E-Mail-Adresse darf maximal 256 Zeichen lang sein.")]
    public string Email { get; set; } = string.Empty;

    [Range(1, 80, ErrorMessage = "Die wöchentliche Sollarbeitszeit muss zwischen 1 und 80 Stunden liegen.")]
    public double WeeklyTargetHours { get; set; } = 40;

    [Range(0, 60, ErrorMessage = "Die Urlaubstage müssen zwischen 0 und 60 liegen.")]
    public int AnnualVacationDays { get; set; } = 30;
}
