using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace InsightMed.Infrastructure.Data;

public sealed class AppDbContext : IdentityDbContext<IdentityUser>, IAppDbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<LabParameter> LabParameters { get; set; }
    public DbSet<LabReport> LabReports { get; set; }
    public DbSet<LabRequest> LabRequests { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Patient> Patients { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options) =>
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LabReport>()
            .HasOne(lr => lr.LabRequest)
            .WithOne(lrq => lrq.LabReport)
            .HasForeignKey<LabReport>(lr => lr.LabRequestId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<LabReport>()
            .HasOne(lr => lr.Patient)
            .WithMany(p => p.LabReports)
            .HasForeignKey(lr => lr.PatientId)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<LabRequest>()
            .HasOne(lr => lr.Patient)
            .WithMany(p => p.LabRequests)
            .HasForeignKey(lr => lr.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.LabReport)
            .WithOne(lr => lr.Notification)
            .HasForeignKey<Notification>(n => n.LabReportId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(n => n.RequesterId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}