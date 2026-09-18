using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgendaaS.Application.DTOs;
using AgendaaS.Application.Interfaces;
using AgendaaS.Application.Mapping;
using AgendaaS.Domain.Entities;

namespace AgendaaS.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/clients")]
    public class ClientsController : BaseApiController
    {
        private readonly IApplicationDbContext _context;
        public ClientsController(IApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok((await _context.Clients.ToListAsync()).Select(c => c.ToResponse()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
            return client is null ? NotFound() : Ok(client.ToResponse());
        }

        [HttpPost]
        [Authorize(Roles = "TenantOwner,Receptionist")]
        public async Task<IActionResult> Create(CreateClientRequest request)
        {
            var client = new Client
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Phone = request.Phone,
                Email = request.Email
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client.ToResponse());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "TenantOwner,Receptionist")]
        public async Task<IActionResult> Update(Guid id, CreateClientRequest request)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
            if (client is null) return NotFound();

            client.Name = request.Name;
            client.Phone = request.Phone;
            client.Email = request.Email;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
