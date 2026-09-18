using Microsoft.EntityFrameworkCore;
using AgendaaS.Domain.Entities;
using AgendaaS.Application.Interfaces;

namespace AgendaaS.Infrastructure.Data
{
    public class AppDbContext : DbContext, IApplicationDbContext
    {
        private readonly ITenantProvider _tenantProvider;

        public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenantProvider)
            : base(options)
        {
            _tenantProvider = tenantProvider;
        }

        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Professional> Professionals => Set<Professional>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Professional)
                .WithMany()
                .HasForeignKey(u => u.ProfessionalId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Professional)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.ProfessionalId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany()
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Client)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.ProfessionalId, a.StartTime });

            modelBuilder.Entity<Tenant>()
                .HasIndex(t => t.Slug)
                .IsUnique();

            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<User>().HasQueryFilter(e => e.TenantId == _tenantProvider.GetTenantId());
            modelBuilder.Entity<Professional>().HasQueryFilter(e => e.TenantId == _tenantProvider.GetTenantId());
            modelBuilder.Entity<Service>().HasQueryFilter(e => e.TenantId == _tenantProvider.GetTenantId());
            modelBuilder.Entity<Client>().HasQueryFilter(e => e.TenantId == _tenantProvider.GetTenantId());
            modelBuilder.Entity<Appointment>().HasQueryFilter(e => e.TenantId == _tenantProvider.GetTenantId());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            StampTenantId();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            StampTenantId();
            return base.SaveChanges();
        }

        private void StampTenantId()
        {
            var tenantId = _tenantProvider.GetTenantId();
            if (tenantId is null) return;

            foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
            {
                switch (entry.Entity)
                {
                    case User u when u.TenantId == Guid.Empty: u.TenantId = tenantId.Value; break;
                    case Professional p when p.TenantId == Guid.Empty: p.TenantId = tenantId.Value; break;
                    case Service s when s.TenantId == Guid.Empty: s.TenantId = tenantId.Value; break;
                    case Client c when c.TenantId == Guid.Empty: c.TenantId = tenantId.Value; break;
                    case Appointment a when a.TenantId == Guid.Empty: a.TenantId = tenantId.Value; break;
                }
            }
        }
    }
}
