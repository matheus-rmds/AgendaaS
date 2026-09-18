namespace AgendaaS.Domain.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Professional> Professionals { get; set; } = new List<Professional>();
        public ICollection<Service> Services { get; set; } = new List<Service>();
        public ICollection<Client> Clients { get; set; } = new List<Client>();
    }
}
