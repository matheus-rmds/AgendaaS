using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace AgendaaS.Api.Controllers
{
    public abstract class BaseApiController : ControllerBase
    {
        protected Guid CurrentUserId => Guid.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        protected string CurrentRole => User.FindFirst(ClaimTypes.Role)!.Value;
    }
}
