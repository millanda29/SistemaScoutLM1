import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './core/auth/auth.guard';
import { LoginComponent } from './core/auth/login/login.component';
import { ForgotPasswordComponent } from './core/auth/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './core/auth/reset-password/reset-password.component';
import { SetPasswordComponent } from './core/auth/set-password/set-password.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'forgot-password', component: ForgotPasswordComponent },
  { path: 'reset-password', component: ResetPasswordComponent },
  { path: 'set-password', component: SetPasswordComponent },
  {
    path: 'dashboard',
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./features/dashboard/dashboard.module').then((m) => m.DashboardModule),
  },
  {
    path: 'profile',
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./features/profile/profile.module').then((m) => m.ProfileModule),
  },
  {
    path: 'resources',
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./features/resources/resources.module').then((m) => m.ResourcesModule),
  },
  {
    path: 'categories',
    canActivate: [AuthGuard],
    data: { roles: ['ADMIN', 'SUPERINTENDENTE'] },
    loadChildren: () =>
      import('./features/categories/categories.module').then((m) => m.CategoriesModule),
  },
  {
    path: 'locations',
    canActivate: [AuthGuard],
    data: { roles: ['ADMIN', 'SUPERINTENDENTE'] },
    loadChildren: () =>
      import('./features/locations/locations.module').then((m) => m.LocationsModule),
  },
  {
    path: 'loans',
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./features/loans/loans.module').then((m) => m.LoansModule),
  },
  {
    path: 'maintenance',
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./features/maintenance/maintenance.module').then((m) => m.MaintenanceModule),
  },
  {
    path: 'inventory',
    canActivate: [AuthGuard],
    data: { roles: ['ADMIN', 'SUPERINTENDENTE'] },
    loadChildren: () =>
      import('./features/physical-inventory/physical-inventory.module').then(
        (m) => m.PhysicalInventoryModule
      ),
  },
  {
    path: 'losses',
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./features/losses/losses.module').then((m) => m.LossesModule),
  },
  {
    path: 'retirements',
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./features/retirements/retirements.module').then((m) => m.RetirementsModule),
  },
  {
    path: 'users',
    canActivate: [AuthGuard],
    data: { roles: ['ADMIN', 'SUPERINTENDENTE'] },
    loadChildren: () =>
      import('./features/users/users.module').then((m) => m.UsersModule),
  },
  {
    path: 'dirigentes',
    canActivate: [AuthGuard],
    data: { roles: ['ADMIN', 'SUPERINTENDENTE', 'JEFE_GRUPO', 'DIRIGENTE'] },
    loadChildren: () =>
      import('./features/dirigentes/dirigentes.module').then((m) => m.DirigentesModule),
  },
  {
    path: 'reports',
    canActivate: [AuthGuard],
    data: { roles: ['ADMIN', 'SUPERINTENDENTE', 'JEFE_GRUPO', 'DIRIGENTE'] },
    loadChildren: () =>
      import('./features/reports/reports.module').then((m) => m.ReportsModule),
  },
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: '**', redirectTo: '/dashboard' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
