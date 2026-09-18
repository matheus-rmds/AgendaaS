import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard-layout',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  template: `
    <header class="topbar">
      <span class="brand">AgendaaS</span>
      <nav class="topnav">
        <a routerLink="/dashboard/professionals" routerLinkActive="active">Profissionais</a>
        <a routerLink="/dashboard/services" routerLinkActive="active">Serviços</a>
        <a routerLink="/dashboard/clients" routerLinkActive="active">Clientes</a>
        <a routerLink="/dashboard/appointments" routerLinkActive="active">Agenda</a>
      </nav>
      <button class="btn btn-ghost" (click)="logout()">Sair</button>
    </header>
    <router-outlet></router-outlet>
  `
})
export class DashboardLayoutComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
