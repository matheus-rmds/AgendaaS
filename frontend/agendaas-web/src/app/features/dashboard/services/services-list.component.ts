import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ServiceCatalogService } from '../../../core/services/service-catalog.service';
import { ServiceResponse } from '../../../core/models/service.models';

@Component({
  selector: 'app-services-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './services-list.component.html'
})
export class ServicesListComponent implements OnInit {
  private fb = inject(FormBuilder);
  private serviceCatalogService = inject(ServiceCatalogService);

  services = signal<ServiceResponse[]>([]);
  errorMessage = signal('');

  form = this.fb.group({
    name: ['', Validators.required],
    price: [0, [Validators.required, Validators.min(0)]],
    duration: ['00:30:00', Validators.required]
  });

  ngOnInit(): void {
    this.loadServices();
  }

  loadServices(): void {
    this.serviceCatalogService.getAll().subscribe({
      next: (data) => this.services.set(data),
      error: () => this.errorMessage.set('Erro ao carregar serviços.')
    });
  }

  submit(): void {
    if (this.form.invalid) return;

    this.serviceCatalogService.create(this.form.getRawValue() as any).subscribe({
      next: () => {
        this.form.reset({ price: 0, duration: '00:30:00' });
        this.loadServices();
      },
      error: (err) => this.errorMessage.set(err.error?.error ?? 'Erro ao criar serviço.')
    });
  }

  deactivate(id: string): void {
    this.serviceCatalogService.deactivate(id).subscribe({
      next: () => this.loadServices(),
      error: () => this.errorMessage.set('Erro ao desativar serviço.')
    });
  }
}
