using System.ComponentModel.DataAnnotations;
using AgendaaS.Shared.Enums;

namespace AgendaaS.Application.DTOs
{
    public record CreateAppointmentRequest(Guid ProfessionalId, Guid ServiceId, Guid ClientId, DateTime StartTime);
    public record UpdateAppointmentStatusRequest(AppointmentStatus Status);

    public record CreatePublicAppointmentRequest(
        Guid ProfessionalId, Guid ServiceId, DateTime StartTime,
        [Required, MaxLength(120)] string ClientName,
        [Required, Phone, MaxLength(20)] string ClientPhone,
        [EmailAddress, MaxLength(150)] string? ClientEmail);
}
