namespace AgendaaS.Application.Interfaces
{
    public interface ITenantProvider
    {
        Guid? GetTenantId();
        void SetTenantId(Guid tenantId);
    }
}
