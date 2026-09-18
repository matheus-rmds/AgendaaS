using System.ComponentModel.DataAnnotations;

namespace AgendaaS.Application.DTOs
{
    public record CreateProfessionalRequest(
        [Required, MaxLength(120)] string Name,
        TimeSpan WorkStart,
        TimeSpan WorkEnd);

    public record CreateServiceRequest(
        [Required, MaxLength(120)] string Name,
        [Range(0, 100000)] decimal Price,
        TimeSpan Duration);

    public record CreateClientRequest(
        [Required, MaxLength(120)] string Name,
        [Required, Phone, MaxLength(20)] string Phone,
        [EmailAddress, MaxLength(150)] string? Email);
}
