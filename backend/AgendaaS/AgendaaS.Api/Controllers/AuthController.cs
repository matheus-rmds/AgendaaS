using Microsoft.AspNetCore.Mvc;
using AgendaaS.Application.DTOs;
using AgendaaS.Application.Interfaces;

namespace AgendaaS.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(SignupRequest request)
            => Ok(await _authService.SignupAsync(request));

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
            => Ok(await _authService.LoginAsync(request));
    }
}
