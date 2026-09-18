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
    [Route("api/professionals")]
    public class ProfessionalsController : BaseApiController
    {
        private readonly IApplicationDbContext _context;
        public ProfessionalsController(IApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok((await _context.Professionals.ToListAsync()).Select(p => p.ToResponse()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var professional = await _context.Professionals.FirstOrDefaultAsync(p => p.Id == id);
            return professional is null ? NotFound() : Ok(professional.ToResponse());
        }

        [HttpPost]
        [Authorize(Roles = "TenantOwner")]
        public async Task<IActionResult> Create(CreateProfessionalRequest request)
        {
            var professional = new Professional
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                WorkStart = request.WorkStart,
                WorkEnd = request.WorkEnd
            };

            _context.Professionals.Add(professional);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = professional.Id }, professional.ToResponse());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "TenantOwner")]
        public async Task<IActionResult> Update(Guid id, CreateProfessionalRequest request)
        {
            var professional = await _context.Professionals.FirstOrDefaultAsync(p => p.Id == id);
            if (professional is null) return NotFound();

            professional.Name = request.Name;
            professional.WorkStart = request.WorkStart;
            professional.WorkEnd = request.WorkEnd;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "TenantOwner")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var professional = await _context.Professionals.FirstOrDefaultAsync(p => p.Id == id);
            if (professional is null) return NotFound();

            professional.IsActive = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
