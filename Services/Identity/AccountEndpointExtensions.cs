using AzubiLog.Data;
using AzubiLog.Models;
using AzubiLog.Services.Pdf;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AzubiLog.Services.Identity;

public static class AccountEndpointExtensions
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/account/register/submit", async (
            HttpContext context,
            AccountFlowService accountFlow) =>
        {
            var form = await context.Request.ReadFormAsync();
            var firstName = form["firstName"].ToString();
            var lastName = form["lastName"].ToString();
            var email = form["email"].ToString();
            var password = form["password"].ToString();
            var confirmPassword = form["confirmPassword"].ToString();
            var school = form["school"].ToString();
            var className = form["className"].ToString();
            var role = form["role"].ToString();

            if (string.IsNullOrWhiteSpace(firstName)
                || string.IsNullOrWhiteSpace(lastName)
                || string.IsNullOrWhiteSpace(email)
                || string.IsNullOrWhiteSpace(password)
                || password != confirmPassword
                || string.IsNullOrWhiteSpace(school)
                || string.IsNullOrWhiteSpace(className))
            {
                return Results.Redirect("/account/register?error=invalid");
            }

            var result = await accountFlow.RegisterAsync(
                firstName, lastName, email, password,
                school, className, role, context.RequestAborted);

            return result.Succeeded
                ? Results.Redirect("/account/login?registered=true")
                : Results.Redirect("/account/register?error=failed");
        }).AllowAnonymous().DisableAntiforgery();

        endpoints.MapPost("/account/login/submit", async (
            HttpContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager) =>
        {
            var form = await context.Request.ReadFormAsync();
            var email = form["email"].ToString();
            var password = form["password"].ToString();
            var rememberMe = string.Equals(form["rememberMe"].ToString(), "true", StringComparison.OrdinalIgnoreCase);

            var user = await userManager.FindByEmailAsync(email);
            if (user is null || !await userManager.CheckPasswordAsync(user, password))
            {
                return Results.Redirect("/account/login?error=invalid");
            }

            await signInManager.SignInAsync(user, rememberMe);
            return Results.Redirect("/");
        }).AllowAnonymous().DisableAntiforgery();

        endpoints.MapPost("/account/logout/submit", async (SignInManager<ApplicationUser> signInManager) =>
        {
            await signInManager.SignOutAsync();
            return Results.Redirect("/account/login");
        }).RequireAuthorization().DisableAntiforgery();

        endpoints.MapPost("/account/profile/submit", async (
            HttpContext context,
            ICurrentUserService currentUserService,
            UserManager<ApplicationUser> userManager) =>
        {
            var user = await currentUserService.GetRequiredUserAsync(context.RequestAborted);
            var form = await context.Request.ReadFormAsync();

            user.FirstName = form["firstName"].ToString().Trim();
            user.LastName = form["lastName"].ToString().Trim();
            user.School = form["school"].ToString().Trim();
            user.ClassName = form["className"].ToString().Trim();
            user.TrainingOccupation = form["trainingOccupation"].ToString().Trim();
            user.WeeklyTargetHours = double.TryParse(form["weeklyTargetHours"], out var h) ? h : 40;
            user.AnnualVacationDays = int.TryParse(form["annualVacationDays"], out var d) ? d : 30;
            user.TrainingYear = int.TryParse(form["trainingYear"], out var y) ? y : 1;

            var result = await userManager.UpdateAsync(user);
            return Results.Redirect(result.Succeeded ? "/account/profile?saved=true" : "/account/profile?error=failed");
        }).RequireAuthorization().DisableAntiforgery();

        endpoints.MapPost("/konto/update", async (
            HttpContext context,
            ICurrentUserService currentUserService,
            UserManager<ApplicationUser> userManager) =>
        {
            var user = await currentUserService.GetRequiredUserAsync(context.RequestAborted);
            var form = await context.Request.ReadFormAsync();

            user.FirstName = form["firstName"].ToString().Trim();
            user.LastName = form["lastName"].ToString().Trim();
            user.WeeklyTargetHours = double.TryParse(form["weeklyTargetHours"], out var h) ? h : 40;
            user.AnnualVacationDays = int.TryParse(form["annualVacationDays"], out var d) ? d : 30;

            var newEmail = form["email"].ToString().Trim();
            var currentEmail = user.Email ?? string.Empty;

            if (!string.Equals(currentEmail, newEmail, StringComparison.OrdinalIgnoreCase))
            {
                var existing = await userManager.FindByEmailAsync(newEmail);
                if (existing is not null && existing.Id != user.Id)
                {
                    return Results.Redirect("/konto?error=email-taken");
                }

                var emailResult = await userManager.SetEmailAsync(user, newEmail);
                if (!emailResult.Succeeded)
                {
                    return Results.Redirect("/konto?error=failed");
                }

                await userManager.SetUserNameAsync(user, newEmail);
            }

            var result = await userManager.UpdateAsync(user);
            return Results.Redirect(result.Succeeded ? "/konto?saved=account" : "/konto?error=failed");
        }).RequireAuthorization().DisableAntiforgery();

        endpoints.MapPost("/konto/password", async (
            HttpContext context,
            ICurrentUserService currentUserService,
            UserManager<ApplicationUser> userManager) =>
        {
            var user = await currentUserService.GetRequiredUserAsync(context.RequestAborted);
            var form = await context.Request.ReadFormAsync();

            var currentPassword = form["currentPassword"].ToString();
            var newPassword = form["newPassword"].ToString();
            var confirmPassword = form["confirmPassword"].ToString();

            if (string.IsNullOrWhiteSpace(currentPassword)
                || string.IsNullOrWhiteSpace(newPassword))
            {
                return Results.Redirect("/konto?error=failed");
            }

            if (newPassword != confirmPassword)
            {
                return Results.Redirect("/konto?error=password-mismatch");
            }

            if (!await userManager.CheckPasswordAsync(user, currentPassword))
            {
                return Results.Redirect("/konto?error=password-wrong");
            }

            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                return Results.Redirect("/konto?saved=password");
            }

            return Results.Redirect("/konto?error=password-invalid");
        }).RequireAuthorization().DisableAntiforgery();

        endpoints.MapGet("/api/export/weekly/{year:int}/{week:int}", async (
            int year, int week,
            ICurrentUserService currentUserService,
            ApplicationDbContext dbContext) =>
        {
            var user = await currentUserService.GetRequiredUserAsync();
            var report = await dbContext.WeeklyReports
                .Include(w => w.Entries)
                .FirstOrDefaultAsync(w => w.UserId == user.Id && w.Year == year && w.CalendarWeek == week);

            if (report is null) return Results.NotFound("Wochenbericht nicht gefunden.");

            var pdf = PdfExportService.GenerateWeeklyReport(report, user);
            return Results.File(pdf, "application/pdf", $"Wochenbericht_KW{week}_{year}.pdf");
        }).RequireAuthorization();

        return endpoints;
    }
}
