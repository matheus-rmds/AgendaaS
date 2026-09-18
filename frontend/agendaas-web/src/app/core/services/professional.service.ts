import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateProfessionalRequest, ProfessionalResponse } from '../models/professional.models';

@Injectable({ providedIn: 'root' })
export class ProfessionalService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/professionals`;

  getAll(): Observable<ProfessionalResponse[]> {
    return this.http.get<ProfessionalResponse[]>(this.baseUrl);
  }

  create(request: CreateProfessionalRequest): Observable<ProfessionalResponse> {
    return this.http.post<ProfessionalResponse>(this.baseUrl, request);
  }

  deactivate(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
