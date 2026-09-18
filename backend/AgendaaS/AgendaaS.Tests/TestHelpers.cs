using Microsoft.EntityFrameworkCore;
using AgendaaS.Application.Interfaces;
using AgendaaS.Infrastructure.Data;

namespace AgendaaS.Tests
{
    // Implementação de ITenantProvider controlável pelo teste — troca de "tenant logado"
    // é só mudar essa propriedade, sem precisar de JWT nem HttpContext de verdade.
    public class FakeTenantProvider : ITenantProvider
    {
        public Guid? TenantId { get; set; }
        public Guid? GetTenantId() => TenantId;
        public void SetTenantId(Guid tenantId) => TenantId = tenantId;
    }

    public static class TestDbContextFactory
    {
        // Mesmo dbName = mesmo banco em memória (simula dois contexts acessando os
        // mesmos dados, como dois requests diferentes acessando o mesmo SQL Server).
        public static AppDbContext Create(string dbName, ITenantProvider tenantProvider)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new AppDbContext(options, tenantProvider);
        }
    }
}
