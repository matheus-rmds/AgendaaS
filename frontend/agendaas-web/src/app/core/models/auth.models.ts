export interface SignupRequest {
  salonName: string;
  ownerName: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  name: string;
  role: string;
  tenantId: string;
}
