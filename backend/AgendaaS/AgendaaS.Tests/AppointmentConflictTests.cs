using AgendaaS.Application.Services;
using AgendaaS.Domain.Entities;
using AgendaaS.Shared.Exceptions;
using Xunit;

namespace AgendaaS.Tests
{
    public class AppointmentConflictTests
    {
        [Fact]
        public async Task CreateAppointmentAsync_DeveImpedirSobreposicaoDeHorario()
        {
            var dbName = Guid.NewGuid().ToString();
            var tenantId = Guid.NewGuid();
            var tenantProvider = new FakeTenantProvider { TenantId = tenantId };

            Guid professionalId, serviceId, clientId;

            await using (var context = TestDbContextFactory.Create(dbName, tenantProvider))
            {
                var professional = new Professional
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = "Profissional",
                    WorkStart = new TimeSpan(8, 0, 0),
                    WorkEnd = new TimeSpan(18, 0, 0)
                };
                var service = new Service
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = "Corte",
                    Price = 50,
                    Duration = TimeSpan.FromMinutes(30)
                };
                var client = new Client
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = "Cliente Teste",
                    Phone = "11999999999"
                };

                context.Professionals.Add(professional);
                context.Services.Add(service);
                context.Clients.Add(client);
                await context.SaveChangesAsync();

                professionalId = professional.Id;
                serviceId = service.Id;
                clientId = client.Id;
            }

            await using var context2 = TestDbContextFactory.Create(dbName, tenantProvider);
            var appointmentService = new AppointmentService(context2);

            var primeiroHorario = new DateTime(2026, 8, 25, 10, 0, 0);
            await appointmentService.CreateAppointmentAsync(professionalId, serviceId, clientId, primeiroHorario);

            var horarioSobreposto = new DateTime(2026, 8, 25, 10, 15, 0);

            await Assert.ThrowsAsync<BusinessException>(() =>
                appointmentService.CreateAppointmentAsync(professionalId, serviceId, clientId, horarioSobreposto));
        }
    }
}
