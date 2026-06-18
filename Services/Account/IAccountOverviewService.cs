namespace AzubiLog.Services.Account;

public interface IAccountOverviewService
{
    Task<AccountOverviewViewModel> GetAccountAsync(CancellationToken cancellationToken = default);
    Task SaveAccountAsync(AccountOverviewFormModel model, CancellationToken cancellationToken = default);
    Task<(bool Succeeded, string? ErrorMessage)> ChangePasswordAsync(ChangePasswordFormModel model, CancellationToken cancellationToken = default);
}
