using AzubiLog.Services.Account;
using Microsoft.AspNetCore.Components;

namespace AzubiLog.Components.Pages;

public partial class AccountOverview : ComponentBase
{
    [Inject]
    private IAccountOverviewService AccountService { get; set; } = null!;

    protected AccountOverviewViewModel? ViewModel { get; private set; }
    protected AccountOverviewFormModel AccountForm { get; private set; } = new();
    protected ChangePasswordFormModel PasswordForm { get; private set; } = new();

    protected string? AccountStatusMessage { get; private set; }
    protected string AccountStatusCss { get; private set; } = string.Empty;
    protected bool IsSavingAccount { get; private set; }

    protected string? PasswordStatusMessage { get; private set; }
    protected string PasswordStatusCss { get; private set; } = string.Empty;
    protected bool IsChangingPassword { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        ViewModel = await AccountService.GetAccountAsync();
        AccountForm = ViewModel.Account;
    }

    protected string GetInitials()
    {
        if (ViewModel is null)
        {
            return "?";
        }

        var first = string.IsNullOrWhiteSpace(AccountForm.FirstName)
            ? string.Empty
            : AccountForm.FirstName[..1].ToUpperInvariant();
        var last = string.IsNullOrWhiteSpace(AccountForm.LastName)
            ? string.Empty
            : AccountForm.LastName[..1].ToUpperInvariant();

        var initials = $"{first}{last}".Trim();
        return string.IsNullOrEmpty(initials) ? "?" : initials;
    }

    protected async Task HandleSaveAccountAsync()
    {
        if (IsSavingAccount)
        {
            return;
        }

        IsSavingAccount = true;
        AccountStatusMessage = null;

        try
        {
            await AccountService.SaveAccountAsync(AccountForm);
            ViewModel = await AccountService.GetAccountAsync();
            AccountForm = ViewModel.Account;
            AccountStatusMessage = Localizer["AccountOverviewSaved"];
            AccountStatusCss = string.Empty;
        }
        catch (InvalidOperationException ex)
        {
            AccountStatusMessage = ex.Message;
            AccountStatusCss = "account-status-error";
        }
        finally
        {
            IsSavingAccount = false;
        }
    }

    protected async Task HandleChangePasswordAsync()
    {
        if (IsChangingPassword)
        {
            return;
        }

        IsChangingPassword = true;
        PasswordStatusMessage = null;

        try
        {
            var (succeeded, errorMessage) = await AccountService.ChangePasswordAsync(PasswordForm);

            if (succeeded)
            {
                PasswordStatusMessage = Localizer["AccountOverviewPasswordChanged"];
                PasswordStatusCss = string.Empty;
                PasswordForm = new ChangePasswordFormModel();
            }
            else
            {
                PasswordStatusMessage = errorMessage ?? Localizer["AccountOverviewPasswordChangeFailed"];
                PasswordStatusCss = "account-status-error";
            }
        }
        catch (InvalidOperationException ex)
        {
            PasswordStatusMessage = ex.Message;
            PasswordStatusCss = "account-status-error";
        }
        finally
        {
            IsChangingPassword = false;
        }
    }
}
