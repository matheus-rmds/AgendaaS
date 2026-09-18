using AgendaaS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AgendaaS.Tests
{
    public class TenantIsolationTests
    {
        [Fact]
        public async Task TenantB_NuncaDeveVerDadosDoTenantA()
        {
            var dbName = Guid.NewGuid().ToString();
            var tenantA = Guid.NewGuid();
            var tenantB = Guid.NewGuid();

            // Cria um profissional pertencente ao Tenant A
            var providerA = new FakeTenantProvider { TenantId = tenantA };
            await using (var contextA = TestDbContextFactory.Create(dbName, providerA))
            {
                contextA.Professionals.Add(new Professional
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantA,
                    Name = "Profissional do Tenant A",
                    WorkStart = new TimeSpan(8, 0, 0),
                    WorkEnd = new TimeSpan(18, 0, 0)
                });
                await contextA.SaveChangesAsync();
            }

            // Consulta o MESMO banco, mas "logado" como Tenant B
            var providerB = new FakeTenantProvider { TenantId = tenantB };
            await using var contextB = TestDbContextFactory.Create(dbName, providerB);

            var resultado = await contextB.Professionals.ToListAsync();

            Assert.Empty(resultado); // Tenant B não pode ver nada do Tenant A
        }
    }
}
