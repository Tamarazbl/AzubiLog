using System.ComponentModel.DataAnnotations;

namespace AzubiLog.Services.Account;

public sealed class ChangePasswordFormModel
{
    [Required(ErrorMessage = "Bitte gib dein aktuelles Passwort ein.")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bitte gib ein neues Passwort ein.")]
    [StringLength(128, MinimumLength = 8, ErrorMessage = "Das Passwort muss zwischen 8 und 128 Zeichen lang sein.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bitte bestätige das neue Passwort.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Die Passwörter stimmen nicht überein.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
