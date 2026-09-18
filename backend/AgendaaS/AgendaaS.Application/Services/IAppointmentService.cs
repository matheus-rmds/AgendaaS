using Microsoft.EntityFrameworkCore;
using AgendaaS.Domain.Entities;
using AgendaaS.Application.Interfaces;
using AgendaaS.Shared.Enums;
using AgendaaS.Shared.Exceptions;

namespace AgendaaS.Application.Services
{
    public interface IAppointmentService
    {
        Task<Appointment> CreateAppointmentAsync(Guid professionalId, Guid serviceId, Guid clientId, DateTime requestedStart);
        Task<List<DateTime>> GetAvailableSlotsAsync(Guid professionalId, Guid serviceId, DateTime date); // agora recebe serviceId
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly IApplicationDbContext _context;

        public AppointmentService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Appointment> CreateAppointmentAsync(Guid professionalId, Guid serviceId, Guid clientId, DateTime requestedStart)
        {
            var professional = await _context.Professionals.FirstOrDefaultAsync(p => p.Id == professionalId);
            var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == serviceId);
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);

            if (professional == null) { throw new BusinessException("Profissional não encontrado."); }
            if (service == null) { throw new BusinessException("Serviço não encontrado."); }
            if (client == null) { throw new BusinessException("Cliente não encontrado."); }
            if (!professional.IsActive) { throw new BusinessException("Profissional inativo."); }

            var requestedTime = requestedStart.TimeOfDay;
            if (requestedTime < professional.WorkStart || requestedTime.Add(service.Duration) > professional.WorkEnd)
            {
                throw new BusinessException("Horário fora do expediente do profissional.");
            }
            var requestedEnd = requestedStart.Add(service.Duration);

            bool hasConflict = await _context.Appointments
                .AnyAsync(a =>
                    a.ProfessionalId == professionalId &&
                    a.Status == AppointmentStatus.Confirmed &&
                    a.StartTime < requestedEnd &&
                    a.EndTime > requestedStart
                );

            if (hasConflict) { throw new BusinessException("Já existe um agendamento neste horário"); }

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                TenantId = professional.TenantId,
                ProfessionalId = professionalId,
                ServiceId = serviceId,
                ClientId = clientId,
                StartTime = requestedStart,
                EndTime = requestedEnd,
                Status = AppointmentStatus.Confirmed,
                CreatedAt = DateTime.UtcNow
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return appointment;
        }

        public async Task<List<DateTime>> GetAvailableSlotsAsync(Guid professionalId, Guid serviceId, DateTime date)
        {
            var professional = await _context.Professionals.FirstOrDefaultAsync(p => p.Id == professionalId);
            var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == serviceId);

            if (professional == null) { throw new BusinessException("Profissional não encontrado"); }
            if (service == null) { throw new BusinessException("Serviço não encontrado"); }

            var dayStart = date.Date;
            var dayEnd = dayStart.AddDays(1);

            var appointments = await _context.Appointments
                .Where(a =>
                    a.ProfessionalId == professionalId &&
                    a.Status == AppointmentStatus.Confirmed &&
                    a.StartTime >= dayStart &&
                    a.StartTime < dayEnd)
                .OrderBy(a => a.StartTime)
                .ToListAsync();

            var slots = new List<DateTime>();
            var currentSlot = date.Date.Add(professional.WorkStart);
            var workEnd = date.Date.Add(professional.WorkEnd);
            var duration = service.Duration;

            while (currentSlot.Add(duration) <= workEnd)
            {
                bool isFree = !appointments.Any(a =>
                    a.StartTime < currentSlot.Add(duration) &&
                    a.EndTime > currentSlot);

                if (isFree) { slots.Add(currentSlot); }
                currentSlot = currentSlot.Add(duration);
            }

            return slots;
        }
    }
}
