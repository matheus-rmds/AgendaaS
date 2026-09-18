import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateServiceRequest, ServiceResponse } from '../models/service.models';

@Injectable({ providedIn: 'root' })
export class ServiceCatalogService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/services`;

  getAll(): Observable<ServiceResponse[]> {
    return this.http.get<ServiceResponse[]>(this.baseUrl);
  }

  create(request: CreateServiceRequest): Observable<ServiceResponse> {
    return this.http.post<ServiceResponse>(this.baseUrl, request);
  }

  deactivate(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
