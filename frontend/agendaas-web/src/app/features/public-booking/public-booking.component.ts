import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { PublicBookingService } from '../../core/services/public-booking.service';
import { PublicAppointmentResponse, PublicTenantInfo } from '../../core/models/public-booking.models';
import { PhoneMaskDirective } from '../../core/../shared/directives/phone-mask.directive';

@Component({
  selector: 'app-public-booking',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, PhoneMaskDirective],
  templateUrl: './public-booking.component.html'
})
export class PublicBookingComponent {
  private route = inject(ActivatedRoute);
  private bookingService = inject(PublicBookingService);
  private fb = inject(FormBuilder);

  slug = this.route.snapshot.paramMap.get('slug')!;

  tenantInfo = signal<PublicTenantInfo | null>(null);
  availableSlots = signal<string[]>([]);
  selectedSlot = signal('');
  errorMessage = signal('');
  confirmedAppointment = signal<PublicAppointmentResponse | null>(null);

  searchForm = this.fb.group({
    serviceId: ['', Validators.required],
    professionalId: ['', Validators.required],
    date: ['', Validators.required]
  });

  clientForm = this.fb.group({
    clientName: ['', Validators.required],
    clientPhone: ['', Validators.required],
    clientEmail: ['']
  });

  constructor() {
    this.bookingService.getTenantInfo(this.slug).subscribe({
      next: (data) => this.tenantInfo.set(data),
      error: () => this.errorMessage.set('Estabelecimento não encontrado.')
    });

    this.searchForm.valueChanges.subscribe(() => this.searchSlots());
  }

  searchSlots(): void {
    if (this.searchForm.invalid) return;

    const { serviceId, professionalId, date } = this.searchForm.getRawValue();
    this.selectedSlot.set('');

    this.bookingService.getAvailableSlots(this.slug, professionalId!, serviceId!, date!).subscribe({
      next: (slots) => this.availableSlots.set(slots),
      error: () => this.errorMessage.set('Erro ao buscar horários disponíveis.')
    });
  }

  confirmBooking(): void {
    if (!this.selectedSlot() || this.clientForm.invalid) return;

    const { serviceId, professionalId } = this.searchForm.getRawValue();
    const { clientName, clientPhone, clientEmail } = this.clientForm.getRawValue();

    this.bookingService.createAppointment(this.slug, {
      professionalId: professionalId!,
      serviceId: serviceId!,
      startTime: this.selectedSlot(),
      clientName: clientName!,
      clientPhone: clientPhone!,
      clientEmail: clientEmail || null
    }).subscribe({
      next: (appointment) => {
        this.confirmedAppointment.set(appointment);
        this.errorMessage.set('');
      },
      error: (err) => this.errorMessage.set(err.error?.error ?? 'Erro ao confirmar agendamento.')
    });
  }
}
