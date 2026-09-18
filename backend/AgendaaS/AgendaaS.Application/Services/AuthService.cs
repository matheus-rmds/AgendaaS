using Microsoft.EntityFrameworkCore;
using AgendaaS.Application.DTOs;
using AgendaaS.Application.Interfaces;
using AgendaaS.Domain.Entities;
using AgendaaS.Shared.Enums;
using AgendaaS.Shared.Exceptions;

namespace AgendaaS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthService(IApplicationDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> SignupAsync(SignupRequest request)
        {
            var slug = Slugify(request.SalonName);

            if (await _context.Tenants.AnyAsync(t => t.Slug == slug))
                throw new BusinessException("Já existe um estabelecimento com esse nome.");

            if (await _context.Users.IgnoreQueryFilters().AnyAsync(u => u.Email == request.Email))
                throw new BusinessException("Este e-mail já está em uso.");

            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = request.SalonName,
                Slug = slug,
                CreatedAt = DateTime.UtcNow
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Name = request.OwnerName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = UserRole.TenantOwner
            };

            _context.Tenants.Add(tenant);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user);
            return new AuthResponse(token, user.Name, user.Role.ToString(), tenant.Id);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new BusinessException("E-mail ou senha inválidos.");

            var token = _tokenService.GenerateToken(user);
            return new AuthResponse(token, user.Name, user.Role.ToString(), user.TenantId);
        }

        private static string Slugify(string input)
        {
            var normalized = input.Trim().ToLowerInvariant();
            var sb = new System.Text.StringBuilder();
            foreach (var c in normalized)
            {
                if (char.IsLetterOrDigit(c)) sb.Append(c);
                else if (c == ' ') sb.Append('-');
            }
            return sb.ToString();
        }
    }
}
