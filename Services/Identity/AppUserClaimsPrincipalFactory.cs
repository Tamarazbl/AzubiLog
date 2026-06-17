using System.Security.Claims;
using AzubiLog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AzubiLog.Services.Identity;

public sealed class AppUserClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>(userManager, roleManager, optionsAccessor)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim("FirstName", user.FirstName));
        identity.AddClaim(new Claim("LastName", user.LastName));
        identity.AddClaim(new Claim("School", user.School));
        identity.AddClaim(new Claim("ClassName", user.ClassName));
        return identity;
    }
}
