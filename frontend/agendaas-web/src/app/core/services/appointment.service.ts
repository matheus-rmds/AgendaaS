import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AppointmentResponse, CreateAppointmentRequest, UpdateAppointmentStatusRequest } from '../models/appointment.models';

@Injectable({ providedIn: 'root' })
export class AppointmentService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/appointments`;

  getAll(): Observable<AppointmentResponse[]> {
    return this.http.get<AppointmentResponse[]>(this.baseUrl);
  }

  create(request: CreateAppointmentRequest): Observable<AppointmentResponse> {
    return this.http.post<AppointmentResponse>(this.baseUrl, request);
  }

  updateStatus(id: string, request: UpdateAppointmentStatusRequest): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${id}/status`, request);
  }
}
