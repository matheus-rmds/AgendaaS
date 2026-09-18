export interface ServiceResponse {
  id: string;
  name: string;
  price: number;
  duration: string;
  isActive: boolean;
}

export interface CreateServiceRequest {
  name: string;
  price: number;
  duration: string;
}
