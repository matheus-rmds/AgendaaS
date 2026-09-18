using AgendaaS.Shared.Enums;

namespace AgendaaS.Domain.Entities
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid ProfessionalId { get; set; }
        public Guid ServiceId { get; set; }
        public Guid ClientId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Confirmed; // CORRIGIDO: era Completed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Tenant Tenant { get; set; } = null!;
        public Professional Professional { get; set; } = null!;
        public Service Service { get; set; } = null!;
        public Client Client { get; set; } = null!;
    }
}
