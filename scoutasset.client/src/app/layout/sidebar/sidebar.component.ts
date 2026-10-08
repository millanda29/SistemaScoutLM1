import { Component, EventEmitter, Output } from '@angular/core';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  standalone: false,
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css'],
})
export class SidebarComponent {
  @Output() navigate = new EventEmitter<void>();

  constructor(public auth: AuthService) {}

  menuItems = [
    { path: '/dashboard', label: 'Dashboard', icon: 'dashboard', roles: [] },
    { path: '/profile', label: 'Mi Perfil', icon: 'account_circle', roles: [] },
    { path: '/resources', label: 'Recursos', icon: 'inventory_2', roles: [] },
    { path: '/categories', label: 'Categorías', icon: 'category', roles: ['ADMIN', 'SUPERINTENDENTE'] },
    { path: '/locations', label: 'Ubicaciones', icon: 'location_on', roles: ['ADMIN', 'SUPERINTENDENTE'] },
    { path: '/loans', label: 'Préstamos', icon: 'handshake', roles: [] },
    { path: '/maintenance', label: 'Mantenimiento', icon: 'build', roles: [] },
    { path: '/users', label: 'Usuarios', icon: 'people', roles: ['ADMIN', 'SUPERINTENDENTE'] },
    { path: '/dirigentes', label: 'Dirigentes', icon: 'supervisor_account', roles: ['ADMIN', 'SUPERINTENDENTE', 'JEFE_GRUPO', 'DIRIGENTE'] },
    { path: '/inventory', label: 'Inventario', icon: 'assignment', roles: ['ADMIN', 'SUPERINTENDENTE'] },
    { path: '/losses', label: 'Pérdidas', icon: 'report_problem', roles: [] },
    { path: '/retirements', label: 'Bajas', icon: 'delete_forever', roles: [] },
    { path: '/reports', label: 'Reportes', icon: 'assessment', roles: ['ADMIN', 'SUPERINTENDENTE', 'JEFE_GRUPO', 'DIRIGENTE'] },
  ];

  showItem(roles: string[]): boolean {
    if (roles.length === 0) return true;
    return roles.some((r) => this.auth.hasRole(r));
  }
}
