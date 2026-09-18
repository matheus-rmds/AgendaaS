using Microsoft.EntityFrameworkCore;
using AgendaaS.Domain.Entities;

namespace AgendaaS.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Tenant> Tenants { get; }
        DbSet<User> Users { get; }
        DbSet<Professional> Professionals { get; }
        DbSet<Service> Services { get; }
        DbSet<Client> Clients { get; }
        DbSet<Appointment> Appointments { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
