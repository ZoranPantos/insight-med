using InsightMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace InsightMed.Integration.Data;

public sealed class AppDbContext : DbContext
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
        modelBuilder.Entity<LabReport>()
            .HasOne(lr => lr.LabRequest)
            .WithOne(lrq => lrq.LabReport)
            .HasForeignKey<LabReport>(lr => lr.LabRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LabReport>()
            .HasOne(lr => lr.Patient)
            .WithMany(p => p.LabReports)
            .HasForeignKey(lr => lr.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
