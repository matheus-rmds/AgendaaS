using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgendaaS.Application.DTOs;
using AgendaaS.Application.Interfaces;
using AgendaaS.Application.Mapping;
using AgendaaS.Application.Services;

namespace AgendaaS.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/appointments")]
    public class AppointmentsController : BaseApiController
    {
        private readonly IApplicationDbContext _context;
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IApplicationDbContext context, IAppointmentService appointmentService)
        {
            _context = context;
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DateTime? date, [FromQuery] Guid? professionalId)
        {
            var query = _context.Appointments.AsQueryable();

            if (date.HasValue)
            {
                var dayStart = date.Value.Date;
                var dayEnd = dayStart.AddDays(1);
                query = query.Where(a => a.StartTime >= dayStart && a.StartTime < dayEnd);
            }

            if (professionalId.HasValue)
                query = query.Where(a => a.ProfessionalId == professionalId.Value);

            var appointments = await query.OrderBy(a => a.StartTime).ToListAsync();
            return Ok(appointments.Select(a => a.ToResponse()));
        }

        [HttpPost]
        [Authorize(Roles = "TenantOwner,Receptionist")]
        public async Task<IActionResult> Create(CreateAppointmentRequest request)
        {
            var appointment = await _appointmentService.CreateAppointmentAsync(
                request.ProfessionalId, request.ServiceId, request.ClientId, request.StartTime);

            return Ok(appointment.ToResponse());
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "TenantOwner,Receptionist")]
        public async Task<IActionResult> UpdateStatus(Guid id, UpdateAppointmentStatusRequest request)
        {
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
            if (appointment is null) return NotFound();

            appointment.Status = request.Status;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
