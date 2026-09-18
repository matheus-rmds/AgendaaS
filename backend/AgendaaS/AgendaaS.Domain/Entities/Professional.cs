namespace AgendaaS.Domain.Entities
{
    public class Professional
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public TimeSpan WorkStart { get; set; } = new(8, 0, 0);
        public TimeSpan WorkEnd { get; set; } = new(18, 0, 0);
        public bool IsActive { get; set; } = true;

        public Tenant Tenant { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
