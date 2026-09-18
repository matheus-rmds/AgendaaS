export interface PublicServiceInfo {
  id: string;
  name: string;
  price: number;
  duration: string;
  isActive: boolean;
}

export interface PublicProfessionalInfo {
  id: string;
  name: string;
  workStart: string;
  workEnd: string;
  isActive: boolean;
}

export interface PublicTenantInfo {
  name: string;
  slug: string;
  services: PublicServiceInfo[];
  professionals: PublicProfessionalInfo[];
}

export interface CreatePublicAppointmentRequest {
  professionalId: string;
  serviceId: string;
  startTime: string;
  clientName: string;
  clientPhone: string;
  clientEmail?: string | null;
}

export interface PublicAppointmentResponse {
  id: string;
  professionalId: string;
  serviceId: string;
  clientId: string;
  startTime: string;
  endTime: string;
  status: string;
}
