using System.ComponentModel.DataAnnotations;
using AzubiLog.Models;
using AzubiLog.Services.Identity;
using Microsoft.AspNetCore.Identity;

namespace AzubiLog.Services.Account;

public sealed class AccountOverviewService(
    ICurrentUserService currentUserService,
    UserManager<ApplicationUser> userManager) : IAccountOverviewService
{
    public async Task<AccountOverviewViewModel> GetAccountAsync(CancellationToken cancellationToken = default)
    {
        var user = await currentUserService.GetRequiredUserAsync(cancellationToken);

        return new AccountOverviewViewModel
        {
            Account = new AccountOverviewFormModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                WeeklyTargetHours = user.WeeklyTargetHours <= 0 ? 40 : user.WeeklyTargetHours,
                AnnualVacationDays = user.AnnualVacationDays <= 0 ? 30 : user.AnnualVacationDays
            },
            Password = new ChangePasswordFormModel(),
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            TrainingOccupation = user.TrainingOccupation,
            CompanyName = user.CompanyName,
            TrainingYear = user.TrainingYear,
            EmailConfirmed = user.EmailConfirmed,
            MemberSince = user.LockoutEnd is null ? null : null
        };
    }

    public async Task SaveAccountAsync(
        AccountOverviewFormModel model,
        CancellationToken cancellationToken = default)
    {
        Validator.ValidateObject(model, new ValidationContext(model), validateAllProperties: true);

        var user = await currentUserService.GetRequiredUserAsync(cancellationToken);

        user.FirstName = model.FirstName.Trim();
        user.LastName = model.LastName.Trim();
        user.WeeklyTargetHours = model.WeeklyTargetHours;
        user.AnnualVacationDays = model.AnnualVacationDays;

        var currentEmail = user.Email ?? string.Empty;
        var newEmail = model.Email.Trim();

        if (!string.Equals(currentEmail, newEmail, StringComparison.OrdinalIgnoreCase))
        {
            var emailResult = await userManager.SetEmailAsync(user, newEmail);
            if (!emailResult.Succeeded)
            {
                var errors = string.Join(" ", emailResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errors);
            }

            await userManager.SetUserNameAsync(user, newEmail);
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }
    }

    public async Task<(bool Succeeded, string? ErrorMessage)> ChangePasswordAsync(
        ChangePasswordFormModel model,
        CancellationToken cancellationToken = default)
    {
        Validator.ValidateObject(model, new ValidationContext(model), validateAllProperties: true);

        var user = await currentUserService.GetRequiredUserAsync(cancellationToken);

        var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        if (result.Succeeded)
        {
            return (true, null);
        }

        var errorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
        return (false, errorMessage);
    }
}
