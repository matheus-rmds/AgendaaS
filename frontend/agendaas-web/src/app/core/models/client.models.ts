export interface ClientResponse {
  id: string;
  name: string;
  phone: string;
  email: string | null;
}

export interface CreateClientRequest {
  name: string;
  phone: string;
  email: string | null;
}
