export interface ProfessionalResponse {
  id: string;
  name: string;
  workStart: string;
  workEnd: string;
  isActive: boolean;
}

export interface CreateProfessionalRequest {
  name: string;
  workStart: string;
  workEnd: string;
}
