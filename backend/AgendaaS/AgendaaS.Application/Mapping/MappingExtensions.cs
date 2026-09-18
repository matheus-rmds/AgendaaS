using AgendaaS.Application.DTOs;
using AgendaaS.Domain.Entities;

namespace AgendaaS.Application.Mapping
{
    public static class MappingExtensions
    {
        public static ProfessionalResponse ToResponse(this Professional p) =>
            new(p.Id, p.Name, p.WorkStart, p.WorkEnd, p.IsActive);

        public static ServiceResponse ToResponse(this Service s) =>
            new(s.Id, s.Name, s.Price, s.Duration, s.IsActive);

        public static ClientResponse ToResponse(this Client c) =>
            new(c.Id, c.Name, c.Phone, c.Email);

        public static AppointmentResponse ToResponse(this Appointment a) =>
            new(a.Id, a.ProfessionalId, a.ServiceId, a.ClientId, a.StartTime, a.EndTime, a.Status.ToString());
    }
}
