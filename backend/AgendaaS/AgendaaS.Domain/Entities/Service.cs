namespace AgendaaS.Domain.Entities
{
    public class Service
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsActive { get; set; } = true;

        public Tenant Tenant { get; set; } = null!;
    }
}
