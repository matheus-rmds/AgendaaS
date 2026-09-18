namespace AgendaaS.Application.DTOs
{
    public record ProfessionalResponse(Guid Id, string Name, TimeSpan WorkStart, TimeSpan WorkEnd, bool IsActive);
    public record ServiceResponse(Guid Id, string Name, decimal Price, TimeSpan Duration, bool IsActive);
    public record ClientResponse(Guid Id, string Name, string Phone, string? Email);
    public record AppointmentResponse(Guid Id, Guid ProfessionalId, Guid ServiceId, Guid ClientId, DateTime StartTime, DateTime EndTime, string Status);
}
