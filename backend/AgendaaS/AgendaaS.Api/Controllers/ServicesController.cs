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
    [Route("api/services")]
    public class ServicesController : BaseApiController
    {
        private readonly IApplicationDbContext _context;
        public ServicesController(IApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok((await _context.Services.ToListAsync()).Select(s => s.ToResponse()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
            return service is null ? NotFound() : Ok(service.ToResponse());
        }

        [HttpPost]
        [Authorize(Roles = "TenantOwner")]
        public async Task<IActionResult> Create(CreateServiceRequest request)
        {
            var service = new Service
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Price = request.Price,
                Duration = request.Duration
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = service.Id }, service.ToResponse());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "TenantOwner")]
        public async Task<IActionResult> Update(Guid id, CreateServiceRequest request)
        {
            var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
            if (service is null) return NotFound();

            service.Name = request.Name;
            service.Price = request.Price;
            service.Duration = request.Duration;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "TenantOwner")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
            if (service is null) return NotFound();

            service.IsActive = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
