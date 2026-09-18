import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { ClientService } from '../../../core/services/client.service';
import { ClientResponse } from '../../../core/models/client.models';
import { PhoneMaskDirective } from '../../../shared/directives/phone-mask.directive';

@Component({
  selector: 'app-clients-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, PhoneMaskDirective],
  templateUrl: './clients-list.component.html'
})
export class ClientsListComponent implements OnInit {
  private fb = inject(FormBuilder);
  private clientService = inject(ClientService);

  clients = signal<ClientResponse[]>([]);
  errorMessage = signal('');
  editingId = signal<string | null>(null);

  form = this.fb.group({
    name: ['', Validators.required],
    phone: ['', Validators.required],
    email: ['']
  });

  ngOnInit(): void {
    this.loadClients();
  }

  loadClients(): void {
    this.clientService.getAll().subscribe({
      next: (data) => this.clients.set(data),
      error: () => this.errorMessage.set('Erro ao carregar clientes.')
    });
  }

  startEdit(client: ClientResponse): void {
    this.editingId.set(client.id);
    this.form.setValue({
      name: client.name,
      phone: client.phone,
      email: client.email ?? ''
    });
  }

  cancelEdit(): void {
    this.editingId.set(null);
    this.form.reset();
  }

  submit(): void {
    if (this.form.invalid) return;

    const raw = this.form.getRawValue();
    const request = {
      name: raw.name ?? '',
      phone: raw.phone ?? '',
      email: raw.email || null
    };
    const idBeingEdited = this.editingId();

    const request$: Observable<unknown> = idBeingEdited
      ? this.clientService.update(idBeingEdited, request)
      : this.clientService.create(request);

    request$.subscribe({
      next: () => {
        this.cancelEdit();
        this.loadClients();
      },
      error: (err: any) => this.errorMessage.set(err.error?.error ?? 'Erro ao salvar cliente.')
    });
  }
}
