using AzubiLog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AzubiLog.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole, string>(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ReportEntry> ReportEntries => Set<ReportEntry>();
    public DbSet<TodoItem> Todos => Set<TodoItem>();
    public DbSet<WeeklyReport> WeeklyReports => Set<WeeklyReport>();
    public DbSet<TimetableEntry> TimetableEntries => Set<TimetableEntry>();
    public DbSet<TrainerAssignment> TrainerAssignments => Set<TrainerAssignment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100);
            entity.Property(u => u.School).HasMaxLength(150);
            entity.Property(u => u.ClassName).HasMaxLength(80);
            entity.Property(u => u.TrainingOccupation).HasMaxLength(150);
            entity.Property(u => u.Department).HasMaxLength(150);
            entity.Property(u => u.WeeklyTargetHours).HasDefaultValue(40d);
            entity.Property(u => u.AnnualVacationDays).HasDefaultValue(30);
            entity.Property(u => u.TrainingYear).HasDefaultValue(1);
        });

        builder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.Property(c => c.Name).HasMaxLength(120).IsRequired();
            entity.Property(c => c.ColorHex).HasMaxLength(7).IsRequired();
            entity.HasIndex(c => new { c.UserId, c.Name }).IsUnique();
            entity.HasOne(c => c.User).WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ReportEntry>(entity =>
        {
            entity.ToTable("ReportEntries");
            entity.Property(e => e.DayType).HasMaxLength(40).IsRequired();
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(4000).IsRequired();
            entity.Property(e => e.Note).HasMaxLength(2000);
            entity.Property(e => e.Subject).HasMaxLength(150);
            entity.Property(e => e.OrderNumber).HasMaxLength(80);
            entity.Property(e => e.Status).HasMaxLength(40).IsRequired();
            entity.Property(e => e.Duration).HasPrecision(5, 2);
            entity.HasIndex(e => new { e.UserId, e.Date });
            entity.HasOne(e => e.User).WithMany(u => u.ReportEntries)
                .HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.WeeklyReport).WithMany(w => w.Entries)
                .HasForeignKey(e => e.WeeklyReportId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Category).WithMany()
                .HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<WeeklyReport>(entity =>
        {
            entity.ToTable("WeeklyReports");
            entity.Property(w => w.Status).HasMaxLength(40).IsRequired();
            entity.Property(w => w.Comment).HasMaxLength(2000);
            entity.Property(w => w.TrainerComment).HasMaxLength(2000);
            entity.HasIndex(w => new { w.UserId, w.Year, w.CalendarWeek }).IsUnique();
            entity.HasOne(w => w.User).WithMany(u => u.WeeklyReports)
                .HasForeignKey(w => w.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<TodoItem>(entity =>
        {
            entity.ToTable("Todos");
            entity.Property(t => t.Title).HasMaxLength(200).IsRequired();
            entity.Property(t => t.Description).HasMaxLength(2000);
            entity.Property(t => t.ReviewStatus).HasMaxLength(40).HasDefaultValue("Offen");
            entity.Property(t => t.ReviewComment).HasMaxLength(2000);
            entity.HasOne(t => t.User).WithMany(u => u.Todos)
                .HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(t => t.ReviewedBy).WithMany()
                .HasForeignKey(t => t.ReviewedByUserId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<TimetableEntry>(entity =>
        {
            entity.ToTable("TimetableEntries");
            entity.Property(t => t.Subject).HasMaxLength(150).IsRequired();
            entity.Property(t => t.Teacher).HasMaxLength(150);
            entity.Property(t => t.Room).HasMaxLength(50);
            entity.Property(t => t.School).HasMaxLength(150).IsRequired();
            entity.Property(t => t.ClassName).HasMaxLength(80).IsRequired();
            entity.HasIndex(t => new { t.School, t.ClassName, t.DayOfWeek, t.Period }).IsUnique();
            entity.HasOne(t => t.CreatedBy).WithMany()
                .HasForeignKey(t => t.CreatedByUserId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<TrainerAssignment>(entity =>
        {
            entity.ToTable("TrainerAssignments");
            entity.HasIndex(t => new { t.TrainerId, t.ApprenticeId }).IsUnique();
            entity.HasOne(t => t.Trainer).WithMany(u => u.TrainerAssignments)
                .HasForeignKey(t => t.TrainerId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(t => t.Apprentice).WithMany()
                .HasForeignKey(t => t.ApprenticeId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
