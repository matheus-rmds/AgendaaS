using AgendaaS.Shared.Enums;

namespace AgendaaS.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public Guid? ProfessionalId { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public Professional? Professional { get; set; }
    }
}
