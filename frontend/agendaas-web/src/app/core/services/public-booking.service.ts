import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreatePublicAppointmentRequest, PublicAppointmentResponse, PublicTenantInfo } from '../models/public-booking.models';

@Injectable({ providedIn: 'root' })
export class PublicBookingService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/public`;

  getTenantInfo(slug: string): Observable<PublicTenantInfo> {
    return this.http.get<PublicTenantInfo>(`${this.baseUrl}/${slug}`);
  }

  getAvailableSlots(slug: string, professionalId: string, serviceId: string, date: string): Observable<string[]> {
    const params = new HttpParams()
      .set('professionalId', professionalId)
      .set('serviceId', serviceId)
      .set('date', date);

    return this.http.get<string[]>(`${this.baseUrl}/${slug}/available-slots`, { params });
  }

  createAppointment(slug: string, request: CreatePublicAppointmentRequest): Observable<PublicAppointmentResponse> {
    return this.http.post<PublicAppointmentResponse>(`${this.baseUrl}/${slug}/appointments`, request);
  }
}
