import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ClientResponse, CreateClientRequest } from '../models/client.models';

@Injectable({ providedIn: 'root' })
export class ClientService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/clients`;

  getAll(): Observable<ClientResponse[]> {
    return this.http.get<ClientResponse[]>(this.baseUrl);
  }

  create(request: CreateClientRequest): Observable<ClientResponse> {
    return this.http.post<ClientResponse>(this.baseUrl, request);
  }

  update(id: string, request: CreateClientRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, request);
  }
}
