using AzubiLog.Data;
using AzubiLog.Models;
using Microsoft.AspNetCore.Identity;

namespace AzubiLog.Services.Identity;

public sealed class AccountFlowService(
    UserManager<ApplicationUser> userManager,
    ApplicationDataInitializer dataInitializer)
{
    public async Task<IdentityResult> RegisterAsync(
        string firstName, string lastName, string email, string password,
        string school, string className, string role,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = email.Trim(),
            Email = email.Trim(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            School = school.Trim(),
            ClassName = className.Trim(),
            EmailConfirmed = true, // no email sender in dev
            WeeklyTargetHours = 40,
            AnnualVacationDays = 30,
            IsActive = true,
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded) return result;

        var validRole = role switch
        {
            AppRole.Klassensprecher => AppRole.Klassensprecher,
            AppRole.Ausbilder => AppRole.Ausbilder,
            _ => AppRole.Azubi,
        };
        await userManager.AddToRoleAsync(user, validRole);
        await dataInitializer.EnsureDefaultCategoriesAsync(user.Id, cancellationToken);

        return result;
    }
}
