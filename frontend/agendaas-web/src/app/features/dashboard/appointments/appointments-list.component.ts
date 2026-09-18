import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AppointmentService } from '../../../core/services/appointment.service';
import { ProfessionalService } from '../../../core/services/professional.service';
import { ServiceCatalogService } from '../../../core/services/service-catalog.service';
import { ClientService } from '../../../core/services/client.service';
import { AppointmentResponse } from '../../../core/models/appointment.models';
import { ProfessionalResponse } from '../../../core/models/professional.models';
import { ServiceResponse } from '../../../core/models/service.models';
import { ClientResponse } from '../../../core/models/client.models';

@Component({
  selector: 'app-appointments-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './appointments-list.component.html'
})
export class AppointmentsListComponent implements OnInit {
  private fb = inject(FormBuilder);
  private appointmentService = inject(AppointmentService);
  private professionalService = inject(ProfessionalService);
  private serviceCatalogService = inject(ServiceCatalogService);
  private clientService = inject(ClientService);

  appointments = signal<AppointmentResponse[]>([]);
  professionals = signal<ProfessionalResponse[]>([]);
  services = signal<ServiceResponse[]>([]);
  clients = signal<ClientResponse[]>([]);
  errorMessage = signal('');

  statusOptions = ['Confirmed', 'Canceled', 'Completed', 'NoShow'];

  form = this.fb.group({
    professionalId: ['', Validators.required],
    serviceId: ['', Validators.required],
    clientId: ['', Validators.required],
    startTime: ['', Validators.required]
  });

  ngOnInit(): void {
    this.professionalService.getAll().subscribe(data => this.professionals.set(data));
    this.serviceCatalogService.getAll().subscribe(data => this.services.set(data));
    this.clientService.getAll().subscribe(data => this.clients.set(data));
    this.loadAppointments();
  }

  loadAppointments(): void {
    this.appointmentService.getAll().subscribe({
      next: (data) => this.appointments.set(data),
      error: () => this.errorMessage.set('Erro ao carregar agendamentos.')
    });
  }

  submit(): void {
    if (this.form.invalid) return;

    this.appointmentService.create(this.form.getRawValue() as any).subscribe({
      next: () => {
        this.form.reset();
        this.loadAppointments();
      },
      error: (err) => this.errorMessage.set(err.error?.error ?? 'Erro ao criar agendamento.')
    });
  }

  updateStatus(id: string, status: string): void {
    this.appointmentService.updateStatus(id, { status }).subscribe({
      next: () => this.loadAppointments(),
      error: () => this.errorMessage.set('Erro ao atualizar status.')
    });
  }

  professionalName(id: string): string {
    return this.professionals().find(p => p.id === id)?.name ?? '—';
  }

  serviceName(id: string): string {
    return this.services().find(s => s.id === id)?.name ?? '—';
  }

  clientName(id: string): string {
    return this.clients().find(c => c.id === id)?.name ?? '—';
  }
}
