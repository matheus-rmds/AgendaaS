import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProfessionalService } from '../../../core/services/professional.service';
import { ProfessionalResponse } from '../../../core/models/professional.models';

@Component({
  selector: 'app-professionals-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './professionals-list.component.html'
})
export class ProfessionalsListComponent implements OnInit {
  private fb = inject(FormBuilder);
  private professionalService = inject(ProfessionalService);

  professionals = signal<ProfessionalResponse[]>([]);
  errorMessage = signal('');

  form = this.fb.group({
    name: ['', Validators.required],
    workStart: ['08:00:00', Validators.required],
    workEnd: ['18:00:00', Validators.required]
  });

  ngOnInit(): void {
    this.loadProfessionals();
  }

  loadProfessionals(): void {
    this.professionalService.getAll().subscribe({
      next: (data) => this.professionals.set(data),
      error: () => this.errorMessage.set('Erro ao carregar profissionais.')
    });
  }

  submit(): void {
    if (this.form.invalid) return;

    this.professionalService.create(this.form.getRawValue() as any).subscribe({
      next: () => {
        this.form.reset({ workStart: '08:00:00', workEnd: '18:00:00' });
        this.loadProfessionals();
      },
      error: (err) => this.errorMessage.set(err.error?.error ?? 'Erro ao criar profissional.')
    });
  }

  deactivate(id: string): void {
    this.professionalService.deactivate(id).subscribe({
      next: () => this.loadProfessionals(),
      error: () => this.errorMessage.set('Erro ao desativar profissional.')
    });
  }
}
