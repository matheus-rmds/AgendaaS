using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgendaaS.Application.DTOs;
using AgendaaS.Application.Interfaces;
using AgendaaS.Application.Mapping;
using AgendaaS.Application.Services;
using AgendaaS.Domain.Entities;

namespace AgendaaS.Api.Controllers
{
    [ApiController]
    [Route("api/public/{slug}")]
    public class PublicController : BaseApiController
    {
        private readonly IApplicationDbContext _context;
        private readonly IAppointmentService _appointmentService;

        public PublicController(IApplicationDbContext context, IAppointmentService appointmentService)
        {
            _context = context;
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTenantInfo(string slug)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Slug == slug && t.IsActive);
            if (tenant is null) return NotFound();

            var services = await _context.Services.Where(s => s.IsActive).ToListAsync();
            var professionals = await _context.Professionals.Where(p => p.IsActive).ToListAsync();

            return Ok(new
            {
                tenant.Name,
                tenant.Slug,
                Services = services.Select(s => s.ToResponse()),
                Professionals = professionals.Select(p => p.ToResponse())
            });
        }

        [HttpGet("available-slots")]
        public async Task<IActionResult> GetAvailableSlots(
            string slug, [FromQuery] Guid professionalId, [FromQuery] Guid serviceId, [FromQuery] DateTime date)
        {
            var slots = await _appointmentService.GetAvailableSlotsAsync(professionalId, serviceId, date);
            return Ok(slots);
        }

        [HttpPost("appointments")]
        public async Task<IActionResult> CreateAppointment(string slug, CreatePublicAppointmentRequest request)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Phone == request.ClientPhone);
            if (client is null)
            {
                client = new Client
                {
                    Id = Guid.NewGuid(),
                    Name = request.ClientName,
                    Phone = request.ClientPhone,
                    Email = request.ClientEmail
                };
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
            }

            var appointment = await _appointmentService.CreateAppointmentAsync(
                request.ProfessionalId, request.ServiceId, client.Id, request.StartTime);

            return Ok(appointment.ToResponse());
        }
    }
}
