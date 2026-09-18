import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'signup', loadComponent: () => import('./features/auth/signup/signup.component').then(m => m.SignupComponent) },
  {
    path: 'agendar/:slug',
    loadComponent: () => import('./features/public-booking/public-booking.component').then(m => m.PublicBookingComponent)
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./features/dashboard/dashboard-layout.component').then(m => m.DashboardLayoutComponent),
    children: [
      { path: '', redirectTo: 'professionals', pathMatch: 'full' },
      {
        path: 'professionals',
        loadComponent: () => import('./features/dashboard/professionals/professionals-list.component').then(m => m.ProfessionalsListComponent)
      },
      {
        path: 'services',
        loadComponent: () => import('./features/dashboard/services/services-list.component').then(m => m.ServicesListComponent)
      },
      {
        path: 'clients',
        loadComponent: () => import('./features/dashboard/clients/clients-list.component').then(m => m.ClientsListComponent)
      },
      {
        path: 'appointments',
        loadComponent: () => import('./features/dashboard/appointments/appointments-list.component').then(m => m.AppointmentsListComponent)
      }
    ]
  }
];
