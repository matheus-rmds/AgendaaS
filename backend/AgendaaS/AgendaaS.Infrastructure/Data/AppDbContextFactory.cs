using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using AgendaaS.Application.Interfaces;

namespace AgendaaS.Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var apiProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "AgendaaS.Api");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options, new DesignTimeTenantProvider());
        }

        private class DesignTimeTenantProvider : ITenantProvider
        {
            public Guid? GetTenantId() => null;
            public void SetTenantId(Guid tenantId) { }
        }
    }
}
