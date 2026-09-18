using AgendaaS.Application.Interfaces;

namespace AgendaaS.Infrastructure.Services
{
    public class CurrentTenantService : ITenantProvider
    {
        public Guid? _tenantId;

        public Guid? GetTenantId() => _tenantId;
        public void SetTenantId(Guid tenantId) => _tenantId = tenantId;
    }
}
