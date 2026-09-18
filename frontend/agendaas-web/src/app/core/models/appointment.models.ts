export interface AppointmentResponse {
  id: string;
  professionalId: string;
  serviceId: string;
  clientId: string;
  startTime: string;
  endTime: string;
  status: string;
}

export interface CreateAppointmentRequest {
  professionalId: string;
  serviceId: string;
  clientId: string;
  startTime: string;
}

export interface UpdateAppointmentStatusRequest {
  status: string;
}
