using System.ComponentModel.DataAnnotations;

namespace AgendaaS.Application.DTOs
{
    public record SignupRequest(
        [Required, MaxLength(120)] string SalonName,
        [Required, MaxLength(120)] string OwnerName,
        [Required, EmailAddress, MaxLength(150)] string Email,
        [Required, MinLength(6), MaxLength(72)] string Password);

    public record LoginRequest(
        [Required, EmailAddress, MaxLength(150)] string Email,
        [Required, MaxLength(72)] string Password);

    public record AuthResponse(string Token, string Name, string Role, Guid TenantId);
}
