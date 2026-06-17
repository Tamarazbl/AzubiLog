using AzubiLog.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AzubiLog.Services.Identity;

public sealed class CurrentUserService(
    IHttpContextAccessor httpContextAccessor,
    AuthenticationStateProvider authenticationStateProvider,
    UserManager<ApplicationUser> userManager) : ICurrentUserService
{
    public async Task<ApplicationUser> GetRequiredUserAsync(CancellationToken cancellationToken = default)
    {
        var principal = httpContextAccessor.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated != true)
        {
            principal = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        }

        if (principal?.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedAccessException("Anmeldung erforderlich.");
        }

        var user = await userManager.GetUserAsync(principal)
            ?? throw new UnauthorizedAccessException("Benutzer nicht gefunden.");

        return user;
    }
}
