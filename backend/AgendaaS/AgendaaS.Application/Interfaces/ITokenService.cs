using AgendaaS.Domain.Entities;

namespace AgendaaS.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
