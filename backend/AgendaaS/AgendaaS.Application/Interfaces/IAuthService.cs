using AgendaaS.Application.DTOs;

namespace AgendaaS.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> SignupAsync(SignupRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
