namespace AzubiLog.Services.Account;

public sealed class AccountOverviewViewModel
{
    public AccountOverviewFormModel Account { get; init; } = new();
    public ChangePasswordFormModel Password { get; init; } = new();
    public string FullName { get; init; } = string.Empty;
    public string TrainingOccupation { get; init; } = string.Empty;
    public string CompanyName { get; init; } = string.Empty;
    public int TrainingYear { get; init; } = 1;
    public bool EmailConfirmed { get; init; }
    public DateTimeOffset? MemberSince { get; init; }
}
