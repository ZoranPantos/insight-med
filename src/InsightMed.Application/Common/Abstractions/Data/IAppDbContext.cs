using InsightMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace InsightMed.Application.Common.Abstractions.Data;

public interface IAppDbContext
{
    DbSet<LabReport> LabReports { get; }
    DbSet<LabParameter> LabParameters { get; }
    DbSet<LabRequest> LabRequests { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<Patient> Patients { get; }
    DbSet<UserProfile> UserProfiles { get; set; }

    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}