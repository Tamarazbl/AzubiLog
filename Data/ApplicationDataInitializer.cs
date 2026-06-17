using AzubiLog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AzubiLog.Data;

public sealed class ApplicationDataInitializer(
    ApplicationDbContext dbContext,
    RoleManager<IdentityRole> roleManager)
{
    public async Task InitializeAsync()
    {
        await dbContext.Database.MigrateAsync();

        string[] roles = [AppRole.Azubi, AppRole.Klassensprecher, AppRole.Ausbilder];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public async Task EnsureDefaultCategoriesAsync(string userId, CancellationToken ct = default)
    {
        if (await dbContext.Categories.AnyAsync(c => c.UserId == userId, ct))
            return;

        var defaults = new (string Name, string Color)[]
        {
            ("Betrieb", "#2F855A"),
            ("Berufsschule", "#3182CE"),
            ("Sonstiges", "#D69E2E"),
        };

        foreach (var (name, color) in defaults)
        {
            dbContext.Categories.Add(new Category
            {
                Name = name,
                ColorHex = color,
                UserId = userId,
            });
        }

        await dbContext.SaveChangesAsync(ct);
    }
}
