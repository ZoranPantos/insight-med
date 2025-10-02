using InsightMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace InsightMed.Application.Common.Abstractions;

public interface IAppDbContext
{
    DbSet<LabReport> LabReports { get; }
    DbSet<LabParameter> LabParameters { get; }
    DbSet<LabRequest> LabRequests { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<Patient> Patients { get; }

    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
